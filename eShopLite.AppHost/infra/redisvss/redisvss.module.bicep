@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param principalId string

resource redisvss 'Microsoft.Cache/redisEnterprise@2024-09-01-preview' = {
  name: take('redisvss-${uniqueString(resourceGroup().id)}', 63)
  location: location
  tags: {
    'aspire-resource-name': 'redisvss'
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

resource redisvssDatabase 'Microsoft.Cache/redisEnterprise/databases@2024-09-01-preview' = {
  name: 'default'
  parent: redisvss
  properties:{
    clientProtocol: 'Encrypted'
    port: 10000
    clusteringPolicy: 'EnterpriseCluster'
    modules: [
      {
        name: 'RediSearch'
      }
      {
        name: 'RedisJSON'
      }
    ]
    evictionPolicy: 'NoEviction'
    persistence:{
      aofEnabled: false 
      rdbEnabled: false
    }
  }
}

resource redisAccessPolicyAssignmentName 'Microsoft.Cache/redisEnterprise/databases/accessPolicyAssignments@2024-09-01-preview' = {
  name: take('cachecontributor${uniqueString(resourceGroup().id)}', 24)
  parent: redisvssDatabase
  properties: {
    accessPolicyName: 'default'
    user: {
      objectId: principalId
      }
    }
  }

output connectionString string = '${redisvss.properties.hostName}:10000,ssl=true'
