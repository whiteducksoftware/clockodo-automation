using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace function
{
    public class TimeTriggerClockodo
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("TimeTriggerClockodo")]
        public async Task Run([TimerTrigger("1 */0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var keyvaultName = GetEnvironmentVariable("KEYVAULT_NAME");
            var connectionString = GetEnvironmentVariable("AzureWebJobsStorage");

            var now = DateTime.Now;
            await ClockodoBackupService.BackupAsync(keyvaultName, HttpClient, connectionString,
                now.AddDays(-1).AddMonths(-1), now, $"Daylybackup/{now.ToString($"MM-dd-yyyy-backup")}.csv");
        }

        public static string GetEnvironmentVariable(string environmentVariable)
        {
            return Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.Process);
        }
    }
}

