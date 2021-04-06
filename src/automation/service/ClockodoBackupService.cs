using Azure.Storage.Blobs;
using CsvHelper;
using automation.model;
using Microsoft.AspNetCore.WebUtilities;
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
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CsvHelper.Configuration;

namespace automation.service
{
    static class ClockodoBackupService
    {
        public static async Task BackupAsync(string keyVaultName, HttpClient httpClient, string connectionString,
            DateTime dateTimeSince, DateTime dateTimeUntil, string backupFileName)
        {
            var options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay = TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };
            var secretClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential(), options);

            var apiUser = secretClient.GetSecret("ClockodoApiUser").Value.Value;
            var apiKey = secretClient.GetSecret("ClockodoApiKey").Value.Value;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiUser}:{apiKey}")));

            var requestUri = QueryHelpers.AddQueryString("https://my.clockodo.com/api/entries",
                new Dictionary<string, string>
                {
                    {"time_since", $"{dateTimeSince:yyyy'-'MM'-'dd HH:mm:ss}"},
                    {"time_until", $"{dateTimeUntil:yyyy'-'MM'-'dd HH:mm:ss}"}
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
                using (var csv = new CsvWriter(writer,
                    new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = delimiter}))
                {
                    csv.WriteHeader<EntryModel.Entry>();
                    csv.NextRecord();
                    csv.WriteRecords(tasks);
                }

                return writer.ToString();
            }
        }
    }
}
