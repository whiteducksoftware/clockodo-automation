param tenant string = ''
param service_principal string = ''
param environment string = 'dev'
param location string = 'westeurope'
param storage_sku string = 'Standard_LRS'
param asp_sku string = 'B1'
param retention_days int = 30

var stac_name = 'clock0dev0stac'
var function_app_name = 'clock-dev-funcapp'
var app_insights_name = 'clock-dev-appinsights'
var guid = 'pid-634ee6d0-daae-4676-8dcf-20e9062d36de'

// deployment 
resource deployment 'Microsoft.Resources/deployments@2020-06-01' = {
  name: guid
  properties: {
    mode: 'Incremental'
    template: {
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      resources: []
    }
  }
}

// keyvault
resource akv 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: 'clock-dev-akv'
  location: location
  properties: {
    tenantId: tenant
    softDeleteRetentionInDays: 60
    sku: {
      name: 'standard'
      family: 'A'
    }
    accessPolicies: [
      {
        tenantId: tenant
        objectId: service_principal
        permissions: {
          secrets: [
            'get'
            'list'
            'set'
          ]
        }
      }
      {
        tenantId: tenant
        objectId: reference(function_app.id, '2020-06-01', 'Full').identity.principalId
        permissions: {
          secrets: [
            'get'
            'list'
            'set'
          ]
        }
      } 
    ]
  }
  dependsOn: [
    function_app
  ]
  tags: {
    environment: environment
  }
}

// storage account
resource stac 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: stac_name
  location: location
  kind: 'StorageV2'
  sku: {
    name: storage_sku
  }
  properties: {
    accessTier: 'Cool'
  }
  tags: {
    environment: environment 
  }
}

// function app
resource function_app 'Microsoft.Web/sites@2020-06-01' = {
  name: function_app_name
  location: location
  tags: {
    environment: environment
  }
  kind: 'functionapp'
  dependsOn: [
    stac
    app_insights
  ]
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: concat('DefaultEndpointsProtocol=https;AccountName=', stac_name, ';AccountKey=', listKeys(stac.id,'2019-06-01').keys[0].value)
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(app_insights.id, '2015-05-01').InstrumentationKey
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: concat('DefaultEndpointsProtocol=https;AccountName=', stac_name, ';AccountKey=', listKeys(stac.id,'2019-06-01').keys[0].value)
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(function_app_name)
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// application insights
resource app_insights 'microsoft.insights/components@2015-05-01' = {
  name: app_insights_name
  location: location
  kind: 'web'
  tags: {
    environment: environment
  }
  properties: {
    Application_Type: 'web'
  }
}