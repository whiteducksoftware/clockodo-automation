param resource_prefix string = 'clockautom'

var stac_name = concat(resource_prefix, uniqueString(resourceGroup().id))
var backup_container = 'backups'
var function_app_name = concat(resource_prefix, '-funcapp')
var app_insights_name = concat(resource_prefix, '-appinsights')
var akv_name = concat(resource_prefix, '-akv')
var storage_sku = 'Standard_LRS'
var location = resourceGroup().location
var tenant = subscription().tenantId

// deployment 
resource deployment 'Microsoft.Resources/deployments@2020-06-01' = {
  name: 'pid-634ee6d0-daae-4676-8dcf-20e9062d36de'
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
  name: akv_name
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
}

// blob container
resource blob_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: concat(stac.name, '/default/', backup_container)
  properties: {
    publicAccess: 'None'
  }
  dependsOn: [
    stac
  ]
}

// function app
resource function_app 'Microsoft.Web/sites@2020-06-01' = {
  name: function_app_name
  location: location
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
        {
          name: 'KEYVAULT_NAME'
          value: akv_name
        }
        {
          name: 'CONTAINER_NAME'
          value: backup_container
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
  properties: {
    Application_Type: 'web'
  }
}