using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using automation.service;

namespace automation.Functions
{
    public static class TimeTriggerDay
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("TimeTriggerDay")]
        public static async Task Run([TimerTrigger("0 59 23 * * *")] TimerInfo myTimer, ILogger log)
        {
            var keyVaultName = GetEnvironmentVariable("KEYVAULT_NAME");
            var connectionString = GetEnvironmentVariable("AzureWebJobsStorage");

            var now = DateTime.Now;
            var dateSince = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var dateUntil = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            await ClockodoBackupService.BackupAsync(keyVaultName, HttpClient, connectionString,
                dateSince, dateUntil, $"Daylybackup/{now.ToString($"MM-dd-yyyy-backup")}.csv");
        }

        private static string GetEnvironmentVariable(string environmentVariable)
        {
            return Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }
    }
}
