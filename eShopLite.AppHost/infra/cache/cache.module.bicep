targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param keyVaultName string

@description('')
param UserAssignedIdentityObjectID string

var redisAccessPolicyAssignment = 'redisWebAppAssignment'

resource keyVault_IeF8jZvXV 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource redisCache_enclX3umP 'Microsoft.Cache/redis@2024-03-01' = {
  name: toLower(take('cache${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'cache'
  }
  properties: {
    redisConfiguration:{
      'aad-enabled':'true'
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 3
    }
  }
  identity:{
    type:'SystemAssigned'
  }
}

resource redisAccessPolicyAssignmentName 'Microsoft.Cache/redis/accessPolicyAssignments@2024-03-01' = {
  name: redisAccessPolicyAssignment
  parent: redisCache_enclX3umP
  properties: {
    accessPolicyName: 'Data Owner'
    objectId: UserAssignedIdentityObjectID
    objectIdAlias: 'eShop'
  }
}

resource keyVaultSecret_Ddsc3HjrA 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault_IeF8jZvXV
  name: 'connectionString'
  location: location
  properties: {
    // value: '${redisCache_enclX3umP.properties.hostName},ssl=true,password=${redisCache_enclX3umP.listKeys(redisCache_enclX3umP.apiVersion).primaryKey}'
    value: redisCache_enclX3umP.properties.hostName
  }
}
