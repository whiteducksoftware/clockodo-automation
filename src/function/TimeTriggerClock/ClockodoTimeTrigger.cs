using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TimeTriggerClock.models;
using Azure.Storage.Blobs;

namespace TimeTriggerClock.function
{
    public class ClockodoTimeTrigger
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("ClockodoTimeTrigger")]
        public async Task Run([TimerTrigger("1 */0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var keyvaultName = GetEnvironmentVariable("KEYVAULT_NAME");

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            string secretName = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "secretName")).Value;
            string secretKey = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "secretKey")).Value;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{secretName}:{secretKey}")));

            var httpResponse = await httpClient.GetAsync("https://my.clockodo.com/api/clock");

            var content = await httpResponse.Content.ReadAsStringAsync();

            var tasks = JsonSerializer.Deserialize<EntrieModel.Rootobject>(content);

            /*
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=clockautombbzew2te7qhdg;AccountKey=p+1V5ID0uMQ0nwcee+uloOs2dcM7sB/P8qloRoNMX2XyK8AKkJte9+B8JKyXXoTTbZigMCc7/TiWuJD2u2qykg==;EndpointSuffix=core.windows.net");
            var storageClient = account.CreateCloudTableClient();
            var table = storageClient.GetTableReference("TestData");  
            TableOperation insertOperation = TableOperation.Insert(new EntrieModel());
        
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=clockautombbzew2te7qhdg;AccountKey=p+1V5ID0uMQ0nwcee+uloOs2dcM7sB/P8qloRoNMX2XyK8AKkJte9+B8JKyXXoTTbZigMCc7/TiWuJD2u2qykg==;EndpointSuffix=core.windows.net";
            var containerName = "test";
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            BlobClient blob = container.GetBlobClient("TestblobName");
            blob.Upload(content);
            */
            
            log.LogInformation($"Status Code: {httpResponse.StatusCode} Data: {await httpResponse.Content.ReadAsStringAsync()}");
        }

        public static string GetEnvironmentVariable(string environmentVariable)
        {
            return System.Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }
    }
}
