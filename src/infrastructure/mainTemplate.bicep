param tenant string = ''
param service_principal string = ''
param environment string = 'dev'

resource akv 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: 'clock-dev-akv'
  location: 'westeurope'
  properties: {
    tenantId: tenant
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
    ]
  }
}

resource blob_stac 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'clock0dev0stac'
  location: 'westeurope'
  kind: 'BlobStorage'
  sku: {
    name: 'Standard_LRS'
  }
  tags: {
    environment: environment 
  }
}