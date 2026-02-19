using System;

namespace Codout.Apis.Asaas.Core
{
    public class ApiSettings
    {
        public string AccessToken { get; }
        public string ApplicationName { get; }
        public AsaasEnvironment AsaasEnvironment { get; }
        public TimeSpan TimeOut { get; set; }

        public ApiSettings(string accessToken, string applicationName, AsaasEnvironment asaasEnvironment)
        {
            AccessToken = accessToken;
            ApplicationName = applicationName;
            AsaasEnvironment = asaasEnvironment;
            TimeOut = TimeSpan.FromSeconds(30);
        }
    }
}
