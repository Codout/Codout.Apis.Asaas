using System;

namespace Codout.Apis.Asaas.Core
{
    public class ApiSettings
    {
        public string AccessToken { get; }
        public string ApplicationName { get; }
        public AsaasEnvironment AsaasEnvironment { get; }
        public TimeSpan TimeOut { get; set; }

        /// <summary>
        /// Override opcional da URL base. Quando informado, sobrepoe o ambiente
        /// (uso tipico: apontar para um mock local como o Mockoon). Default: null.
        /// </summary>
        public string BaseUrl { get; }

        public ApiSettings(string accessToken, string applicationName, AsaasEnvironment asaasEnvironment, string baseUrl = null)
        {
            AccessToken = accessToken;
            ApplicationName = applicationName;
            AsaasEnvironment = asaasEnvironment;
            BaseUrl = baseUrl;
            TimeOut = TimeSpan.FromSeconds(30);
        }
    }
}
