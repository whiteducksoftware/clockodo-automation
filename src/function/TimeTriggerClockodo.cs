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
using function.models;
using System.IO;
using System.Globalization;
using CsvHelper;
using Azure.Storage.Blobs;

namespace function
{
    public class TimeTriggerClockodo
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("TimeTriggerClockodo")]
        public async Task Run([TimerTrigger("30 */0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var keyvaultName = GetEnvironmentVariable("KEYVAULT_NAME");
            var containerName = GetEnvironmentVariable("CONTAINER_NAME");
            var connectionString = GetEnvironmentVariable("AzureWebJobsStorage");

            DateTime localDate = DateTime.Now;

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            string secretName = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "secretName")).Value;
            string secretKey = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "secretKey")).Value;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{secretName}:{secretKey}")));

            var httpResponse = await httpClient.GetAsync($"https://my.clockodo.com/api/entries?time_since=2017-01-01 00:00:00&time_until={localDate.ToString("yyyy'-'MM'-'dd HH:mm:ss")}");
            
            var content = await httpResponse.Content.ReadAsStringAsync();

            var entryModel = JsonSerializer.Deserialize<EntryModel.Rootobject>(content);

            var client = new BlobContainerClient(connectionString, containerName);
           
            using (Stream stream = new MemoryStream(Encoding.UTF32.GetBytes(JsonToCsv(entryModel.entries, ","))))
            {
                await client.UploadBlobAsync($"{localDate.ToString("MMMM yyyy HH:mm:ss")}.csv", stream);
            }

            //log.LogInformation($"Status Code: {httpResponse.StatusCode} Data: {await httpResponse.Content.ReadAsStringAsync()}");
        }

        public static string GetEnvironmentVariable(string environmentVariable)
        {
            return System.Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }

        public static string JsonToCsv(EntryModel.Entry[] tasks, string delimiter)
        {
            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = delimiter;

                    csv.WriteHeader<EntryModel.Entry>();

                    csv.NextRecord();

                    csv.WriteRecords<EntryModel.Entry>(tasks);
                }

                return writer.ToString();
            }
        }

    }
}

