@description('The location for the resource(s) to be deployed.')
// param location string = resourceGroup().location

param principalId string

param principalName string

var location = 'australiaeast'

resource sqlserver 'Microsoft.Sql/servers@2021-11-01' = {
  name: take('sqlserver-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      login: principalName
      sid: principalId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    version: '12.0'
  }
  tags: {
    'aspire-resource-name': 'sqlserver'
  }
}

resource sqlFirewallRule_AllowAllAzureIps 'Microsoft.Sql/servers/firewallRules@2021-11-01' = {
  name: 'AllowAllAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
  parent: sqlserver
}

resource ProductContext 'Microsoft.Sql/servers/databases@2021-11-01' = {
  name: 'ProductContext'
  location: location
  parent: sqlserver
}

output sqlServerFqdn string = sqlserver.properties.fullyQualifiedDomainName
