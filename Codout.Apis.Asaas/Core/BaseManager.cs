using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Core.Extension;
using Codout.Apis.Asaas.Core.Interfaces;
using Codout.Apis.Asaas.Core.Response;

namespace Codout.Apis.Asaas.Core
{
    public class BaseManager
    {
        private const string ProductionUrl = "https://api.asaas.com";
        private const string SandboxUrl = "https://api-sandbox.asaas.com";

        private readonly ApiSettings _settings;

        protected BaseManager(ApiSettings settings)
        {
            _settings = settings;
        }

        protected async Task<ResponseObject<T>> PostMultipartFormDataContentAsync<T>(string resource, object payload)
        {
            using var httpClient = BuildHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            using var multipartContent = new MultipartFormDataContent();

            PropertyInfo[] properties = payload.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                var attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                string jsonPropertyName = attr?.Name;
                if (string.IsNullOrEmpty(jsonPropertyName)) jsonPropertyName = prop.Name.FirstCharToLower();

                if (prop.PropertyType.IsAssignableFrom(typeof(List<IAsaasFile>)))
                {
                    List<IAsaasFile> asaasFiles = prop.GetValue(payload) as List<IAsaasFile>;
                    foreach (IAsaasFile asaasFile in asaasFiles)
                    {
                        multipartContent.Add(BuildByteArrayContent(asaasFile), jsonPropertyName, asaasFile.FileName);
                    }
                    continue;
                }

                if (prop.PropertyType == typeof(IAsaasFile))
                {
                    IAsaasFile asaasFile = prop.GetValue(payload) as IAsaasFile;
                    multipartContent.Add(BuildByteArrayContent(asaasFile), jsonPropertyName, asaasFile.FileName);
                    continue;
                }

                multipartContent.Add(new StringContent(prop.GetValue(payload).ToString()), jsonPropertyName);
            }

            var response = await httpClient.PostAsync(BuildApiRoute(resource), multipartContent);

            return await BuildResponseObject<T>(response);
        }

        protected async Task<ResponseObject<T>> PostAsync<T>(string resource, RequestParameters parameters)
        {
            var payload = parameters.Keys.ToDictionary(key => key, key => parameters[key]);

            return await PostAsync<T>(resource, payload);
        }

        protected async Task<ResponseObject<T>> PostAsync<T>(string resource, object payload)
        {
            using var httpClient = BuildHttpClient();

            using var content = new StringContent(
                JsonSerializer.Serialize(payload, JsonSerializerConfiguration.Options),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            var response = await httpClient.PostAsync(BuildApiRoute(resource), content);

            return await BuildResponseObject<T>(response);
        }

        protected async Task<ResponseObject<T>> GetAsync<T>(string resource, string id = null)
        {
            using var httpClient = BuildHttpClient();

            if (!string.IsNullOrEmpty(id))
            {
                resource += $"/{id}";
            }

            var response = await httpClient.GetAsync(BuildApiRoute(resource));

            return await BuildResponseObject<T>(response);
        }

        protected async Task<ResponseList<T>> GetListAsync<T>(string resource, int offset, int limit, RequestParameters parameters = null)
        {
            using var httpClient = BuildHttpClient();

            parameters ??= new RequestParameters();
            parameters.Add("offset", offset);
            parameters.Add("limit", limit);

            resource += parameters.Build();
            var response = await httpClient.GetAsync(BuildApiRoute(resource));

            return await BuildResponseList<T>(response);
        }

        protected async Task<ResponseObject<T>> PutAsync<T>(string resource, object payload)
        {
            using var httpClient = BuildHttpClient();

            using var content = new StringContent(
                JsonSerializer.Serialize(payload, JsonSerializerConfiguration.Options),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            var response = await httpClient.PutAsync(BuildApiRoute(resource), content);

            return await BuildResponseObject<T>(response);
        }

        protected async Task<ResponseObject<T>> DeleteAsync<T>(string resource, string id = null)
        {
            using var httpClient = BuildHttpClient();

            if (!string.IsNullOrEmpty(id))
            {
                resource += $"/{id}";
            }

            var response = await httpClient.DeleteAsync(BuildApiRoute(resource));

            return await BuildResponseObject<T>(response);
        }

        protected virtual HttpClient BuildHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("access_token", _settings.AccessToken);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", _settings.ApplicationName);
            httpClient.BaseAddress = BuildBaseAddress();
            httpClient.Timeout = _settings.TimeOut;

            return httpClient;
        }

        private string BuildApiRoute(string resource)
        {
            return $"/v3{(resource[0] == '/' ? string.Empty : "/")}{resource}";
        }

        private Uri BuildBaseAddress()
        {
            if (_settings.AsaasEnvironment.IsProduction())
            {
                return new Uri(ProductionUrl);
            }

            if (_settings.AsaasEnvironment.IsSandbox())
            {
                return new Uri(SandboxUrl);
            }

            throw new InvalidOperationException("AsaasEnvironment not supported");
        }

        private async Task<ResponseObject<T>> BuildResponseObject<T>(HttpResponseMessage httpResponseMessage)
        {
            string payload = await httpResponseMessage.Content.ReadAsStringAsync();

            return new ResponseObject<T>(httpResponseMessage.StatusCode, payload);
        }

        private async Task<ResponseList<T>> BuildResponseList<T>(HttpResponseMessage httpResponseMessage)
        {
            string payload = await httpResponseMessage.Content.ReadAsStringAsync();

            return new ResponseList<T>(httpResponseMessage.StatusCode, payload);
        }

        private ByteArrayContent BuildByteArrayContent(IAsaasFile asaasFile)
        {
            ByteArrayContent fileContent = new ByteArrayContent(asaasFile.FileContent);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            return fileContent;
        }
    }
}
