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
        public async Task Run([TimerTrigger("1 */0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var keyvaultName = GetEnvironmentVariable("KEYVAULT_NAME");
            var containerName = GetEnvironmentVariable("CONTAINER_NAME");
            var connectionString = GetEnvironmentVariable("AzureWebJobsStorage");

            var localDate = DateTime.Now;

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var ClockodoApiUser = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "ClockodoApiUser")).Value;
            var ClockodoApiKey = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "ClockodoApiKey")).Value;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClockodoApiUser}:{ClockodoApiKey}")));

            var subDate = DateTime.Now.AddDays(-1).AddMonths(-1);

            var httpResponse = await httpClient.GetAsync($"https://my.clockodo.com/api/entries?time_since={subDate.ToString("yyyy'-'MM'-'dd HH:mm:ss")}&time_until={localDate.ToString("yyyy'-'MM'-'dd HH:mm:ss")}");
            var content = await httpResponse.Content.ReadAsStringAsync();

            var entryModel = JsonSerializer.Deserialize<EntryModel.Rootobject>(content);

            var client = new BlobContainerClient(connectionString, "backups");

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonToCsv(entryModel.entries, ","))))
            {
                var rand = new Random();
                var name = ($"Daylybackup/{localDate.ToString($"MM-dd-yyyy-backup")}.csv");
                if (!client.GetBlobClient(name).Exists())
                {
                    //await client.UploadBlobAsync($"{localDate.ToString($"MM-dd-yyyy-backup")}.csv", stream);
                    await client.UploadBlobAsync($"{name}", stream);
                }
                else
                {
                    Console.WriteLine("Backup exists");
                }
            }
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

