targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment (e.g., dev, test, prod)')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = { 'azd-env-name': environmentName }

// Resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: tags
}

// App Service Plan
module appServicePlan './modules/appServicePlan.bicep' = {
  name: 'appServicePlan'
  scope: rg
  params: {
    name: '${abbrs.webServerFarms}${resourceToken}'
    location: location
    tags: tags
    sku: {
      name: 'B1'
      capacity: 1
    }
  }
}

// Azure SQL Server and Database
module sqlServer './modules/sql.bicep' = {
  name: 'sqlServer'
  scope: rg
  params: {
    serverName: '${abbrs.sqlServers}${resourceToken}'
    databaseName: 'rdsventuresdb'
    location: location
    tags: tags
    administratorLogin: 'sqladmin'
    administratorLoginPassword: 'P@ssw0rd${resourceToken}!' // In production, use Key Vault
  }
}

// Storage Account for blobs
module storage './modules/storage.bicep' = {
  name: 'storage'
  scope: rg
  params: {
    name: '${abbrs.storageStorageAccounts}${resourceToken}'
    location: location
    tags: tags
    containerName: 'attachments'
  }
}

// Key Vault
module keyVault './modules/keyVault.bicep' = {
  name: 'keyVault'
  scope: rg
  params: {
    name: '${abbrs.keyVaultVaults}${resourceToken}'
    location: location
    tags: tags
    principalId: principalId
  }
}

// Application Insights
module applicationInsights './modules/applicationInsights.bicep' = {
  name: 'applicationInsights'
  scope: rg
  params: {
    name: '${abbrs.insightsComponents}${resourceToken}'
    location: location
    tags: tags
  }
}

// API App Service
module apiAppService './modules/appService.bicep' = {
  name: 'apiAppService'
  scope: rg
  params: {
    name: '${abbrs.webSitesAppService}api-${resourceToken}'
    location: location
    tags: union(tags, { 'azd-service-name': 'api' })
    appServicePlanId: appServicePlan.outputs.id
    runtimeName: 'dotnetcore'
    runtimeVersion: '8.0'
    appSettings: {
      APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.outputs.connectionString
      ConnectionStrings__DefaultConnection: 'Server=tcp:${sqlServer.outputs.serverName}.database.windows.net,1433;Initial Catalog=${sqlServer.outputs.databaseName};Persist Security Info=False;User ID=${sqlServer.outputs.administratorLogin};Password=${sqlServer.outputs.administratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
      Azure__BlobStorage__ConnectionString: storage.outputs.connectionString
      Azure__BlobStorage__ContainerName: 'attachments'
    }
  }
}

// Frontend App Service
module frontendAppService './modules/appService.bicep' = {
  name: 'frontendAppService'
  scope: rg
  params: {
    name: '${abbrs.webSitesAppService}web-${resourceToken}'
    location: location
    tags: union(tags, { 'azd-service-name': 'frontend' })
    appServicePlanId: appServicePlan.outputs.id
    runtimeName: 'node'
    runtimeVersion: '20-lts'
    appSettings: {
      VITE_API_URL: 'https://${apiAppService.outputs.uri}/api'
    }
  }
}

// Outputs
output AZURE_LOCATION string = location
output AZURE_RESOURCE_GROUP string = rg.name
output API_URL string = apiAppService.outputs.uri
output FRONTEND_URL string = frontendAppService.outputs.uri
output SQL_SERVER_NAME string = sqlServer.outputs.serverName
output SQL_DATABASE_NAME string = sqlServer.outputs.databaseName
output STORAGE_ACCOUNT_NAME string = storage.outputs.name
output KEY_VAULT_NAME string = keyVault.outputs.name
output APPLICATION_INSIGHTS_NAME string = applicationInsights.outputs.name

