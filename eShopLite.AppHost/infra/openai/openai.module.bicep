@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param principalId string

param principalType string

resource openai 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: take('openai-${uniqueString(resourceGroup().id)}', 64)
  location: location
  kind: 'OpenAI'
  properties: {
    customSubDomainName: toLower(take(concat('openai', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: true
  }
  sku: {
    name: 'S0'
  }
  tags: {
    'aspire-resource-name': 'openai'
  }
}

resource openai_CognitiveServicesOpenAIContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(openai.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'a001fd3d-188f-4b5d-821b-7da978bf7442'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'a001fd3d-188f-4b5d-821b-7da978bf7442')
    principalType: principalType
  }
  scope: openai
}

resource chatModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'chatModelDeployment'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4'
      version: '0613'
    }
  }
  sku: {
    name: 'Standard'
    capacity: 8
  }
  parent: openai
}

resource textEmbeddingName 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'textEmbeddingName'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'text-embedding-ada-002'
      version: '2'
    }
  }
  sku: {
    name: 'Standard'
    capacity:30
  }
  parent: openai
  dependsOn: [
    chatModelDeployment
  ]
}

output connectionString string = 'Endpoint=${openai.properties.endpoint}'
output textembeddingname string = textEmbeddingName.name
output gptname string = chatModelDeployment.name