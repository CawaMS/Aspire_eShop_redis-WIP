targetScope = 'resourceGroup'

// @description('')
// param location string = resourceGroup().location

@description('')
param principalId string

@description('')
param principalName string

var location = 'northeurope'


resource sqlServer_YFcCarAEq 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: toLower(take('sqlserver${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'sqlserver'
  }
  properties: {
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: principalName
      sid: principalId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
  }
}

resource sqlFirewallRule_9ocemjyMQ 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  parent: sqlServer_YFcCarAEq
  name: 'AllowAllAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource sqlDatabase_Fyb2MmtgF 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  parent: sqlServer_YFcCarAEq
  name: 'ProductContext'
  location: location
  properties: {
  }
}

output sqlServerFqdn string = sqlServer_YFcCarAEq.properties.fullyQualifiedDomainName
