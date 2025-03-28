@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param principalId string

//param principalName string

resource cache 'Microsoft.Cache/redisEnterprise@2024-09-01-preview' = {
  name: take('cache-${uniqueString(resourceGroup().id)}', 63)
  location: location
  tags: {
    'aspire-resource-name': 'cache'
  }
  sku: {
    name: 'Balanced_B5'
  }
  identity: {
    type:'SystemAssigned'
  }
  properties: {
    minimumTlsVersion: '1.2'
  }
}

resource redisEnterpriseDatabase 'Microsoft.Cache/redisEnterprise/databases@2024-09-01-preview' = {
  name: 'default'
  parent: cache
  properties:{
    clientProtocol: 'Encrypted'
    port: 10000
    clusteringPolicy: 'OSSCluster'
    evictionPolicy: 'NoEviction'
    persistence:{
      aofEnabled: false 
      rdbEnabled: false
    }
  }
}


resource redisAccessPolicyAssignmentName 'Microsoft.Cache/redisEnterprise/databases/accessPolicyAssignments@2024-09-01-preview' = {
  name: take('cachecontributor${uniqueString(resourceGroup().id)}', 24)
  parent: redisEnterpriseDatabase
  properties: {
    accessPolicyName: 'default'
    user: {
      objectId: principalId
      }
    }
  }

output connectionString string = '${cache.properties.hostName}:10000,ssl=true'
