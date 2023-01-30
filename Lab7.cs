using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xrm.Tools.WebAPI;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xrm.Tools.WebAPI.Results;
using System.Dynamic;
using Xrm.Tools.WebAPI.Requests;
using System.Collections.Generic;

namespace InspectionManagementApp
{
    public class InspectionRouter
    {
        //change the Function1 name to Azure created Function name
        [FunctionName("InspectionRouter")]
        public void Run([TimerTrigger("0 0 0 * * * ")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            CRMWebAPI api = GetCRMWebAPI(log).Result;
            dynamic whoami = api.ExecuteFunction("WhoAmI").Result;
            log.LogInformation($"UserID: {whoami.UserId}");

        }
        //Custom Code Starts here
        private static async Task<CRMWebAPI> GetCRMWebAPI(ILogger log)
        {
            var clientID = Environment.GetEnvironmentVariable("cdsclientid", EnvironmentVariableTarget.Process);
            var clientSecret = Environment.GetEnvironmentVariable("cdsclientsecret", EnvironmentVariableTarget.Process);
            var crmBaseUrl = Environment.GetEnvironmentVariable("cdsurl", EnvironmentVariableTarget.Process);
            var crmurl = crmBaseUrl + "/api/data/v9.2/";
            log.LogInformation(crmurl);
            AuthenticationParameters ap = await AuthenticationParameters.CreateFromUrlAsync(new Uri(crmurl));
            var clientcred = new ClientCredential(clientID, clientSecret);
            // CreateFromUrlAsync returns endpoint while AuthenticationContext expects authority
            // workaround is to downgrade adal to v3.19 or to strip the tail
            var auth = ap.Authority.Replace("/oauth2/authorize", "");
            var authContext = new AuthenticationContext(auth);
            var authenticationResult = await authContext.AcquireTokenAsync(crmBaseUrl, clientcred);

            return new CRMWebAPI(crmurl, authenticationResult.AccessToken);           
        }        
    }
}
