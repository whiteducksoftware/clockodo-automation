using Azure.Storage.Blobs;
using CsvHelper;
using automation.model;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace automation.service
{
    static class ClockodoBackupService
    {
        public static async Task BackupAsync(string keyvaultName, HttpClient httpClient, string connectionString, DateTime dateTimeSince, DateTime dateTimeUntil, string backupFileName)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var clockodoApiUser = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "ClockodoApiUser")).Value;
            var clockodoApiKey = (await kv.GetSecretAsync($"https://{keyvaultName}.vault.azure.net/", "ClockodoApiKey")).Value;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clockodoApiUser}:{clockodoApiKey}")));

            var requestUri = QueryHelpers.AddQueryString("https://my.clockodo.com/api/entries",
                new Dictionary<string, string>
                {
                    { "time_since", $"{dateTimeSince:yyyy'-'MM'-'dd HH:mm:ss}"},
                    { "time_until", $"{dateTimeUntil:yyyy'-'MM'-'dd HH:mm:ss}"}
                });

            var strDecoded = WebUtility.UrlDecode(requestUri);
            
            var httpResponse = await httpClient.GetAsync(strDecoded);
            var content = await httpResponse.Content.ReadAsStringAsync();

            var entryModel = JsonSerializer.Deserialize<EntryModel.Rootobject>(content);

            var client = new BlobContainerClient(connectionString, "backups");

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonToCsv(entryModel.entries, ","))))
            {

                if (!client.GetBlobClient(backupFileName).Exists())
                {
                    await client.UploadBlobAsync($"{backupFileName}", stream);
                }
                else
                {
                    Console.WriteLine("Backup exists");
                }
            }
        }

        public static string JsonToCsv(EntryModel.Entry[] tasks, string delimiter)
        {
            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    delimiter = csv.Configuration.Delimiter;
                    csv.WriteHeader<EntryModel.Entry>();
                    csv.NextRecord();
                    csv.WriteRecords(tasks);
                }
                return writer.ToString();
            }
        }
    }
}
