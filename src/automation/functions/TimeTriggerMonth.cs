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
            var keyvaultName = GetEnvironmentVariable("KEYVAULT_NAME");
            var connectionString = GetEnvironmentVariable("AzureWebJobsStorage");

            var now = DateTime.Now;
            await ClockodoBackupService.BackupAsync(keyvaultName, HttpClient, connectionString,
                now.AddDays(-1).AddMonths(-1), now, $"Monthlybackup/{now.ToString($"MM-dd-yyyy-backup")}.csv");
        }

        public static string GetEnvironmentVariable(string environmentVariable)
        {
            return Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }
    }
}
