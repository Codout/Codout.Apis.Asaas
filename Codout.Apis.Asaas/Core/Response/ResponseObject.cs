using System.Net;
using System.Text.Json;
using Codout.Apis.Asaas.Core.Response.Base;

namespace Codout.Apis.Asaas.Core.Response;

public class ResponseObject<T> : BaseResponse
{
    public T Data { get; }

    public ResponseObject(HttpStatusCode httpStatusCode, string content) : base(httpStatusCode, content)
    {
        if (httpStatusCode != HttpStatusCode.OK) return;

        Data = JsonSerializer.Deserialize<T>(content, JsonSerializerConfiguration.Options);
    }
}
