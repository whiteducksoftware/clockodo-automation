using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class ClockodoTimeTrigger
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("ClockodoTimeTrigger")]
        public async Task Run([TimerTrigger("1 */0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var name = GetEnvironmentVariable("FUNCTIONS_EXTENSION_VERSION");

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            string secretName = (await kv.GetSecretAsync("https://clock-dev-akv.vault.azure.net/", "secretName")).Value;
            string secretKey = (await kv.GetSecretAsync("https://clock-dev-akv.vault.azure.net/", "secretKey")).Value;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{secretName}:{secretKey}")));

            var httpResponse = await httpClient.GetAsync("https://my.clockodo.com/api/clock");

            log.LogInformation($"Status Code: {httpResponse.StatusCode} Data: {await httpResponse.Content.ReadAsStringAsync()} Name {name}");
        }

        public static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
