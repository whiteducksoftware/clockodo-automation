using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using automation.service;
using System.Threading.Tasks;

namespace automation
{
    public class TimeTriggerMonth
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("TimeTriggerMonth")]
        public async Task Run([TimerTrigger("10 */0 * * * *")] TimerInfo myTimer, ILogger log)
        {
           var keyVaultName = GetEnvironmentVariable("KEYVAULT_NAME");
           var connectionString = GetEnvironmentVariable("AzureWebJobsStorage");

           var now = DateTime.Now;
           var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
           var dateSince = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
           var dateUntil = new DateTime(now.Year, now.Month, daysInMonth, 23, 59, 59);

           await ClockodoBackupService.BackupAsync(keyVaultName, HttpClient, connectionString,
               dateSince, dateUntil, $"Monthlybackup/{now.ToString($"MM-yyyy-backup")}.csv");
        }

        public static string GetEnvironmentVariable(string environmentVariable)
        {
           return Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }
    }
}
