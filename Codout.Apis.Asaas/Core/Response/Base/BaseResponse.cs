using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Codout.Apis.Asaas.Core.Extension;

namespace Codout.Apis.Asaas.Core.Response.Base;

public abstract class BaseResponse
{
    public HttpStatusCode StatusCode { get; }

    public List<Error> Errors { get; private set; } = [];

    public string AsaasResponse { get; }

    protected BaseResponse(HttpStatusCode httpStatusCode, string content)
    {
        StatusCode = httpStatusCode;
        AsaasResponse = content;
        BuildErrors();
    }

    private void BuildErrors()
    {
        if (WasSucessfull() || string.IsNullOrEmpty(AsaasResponse))
            return;

        try
        {
            using var document = JsonDocument.Parse(AsaasResponse);

            if (document.RootElement.TryGetProperty("errors", out var errorsElement))
            {
                var errors = JsonSerializer.Deserialize<List<Error>>(errorsElement.GetRawText(), JsonSerializerConfiguration.Options);
                if (errors is not null)
                {
                    Errors = errors;
                }
            }
        }
        catch
        {
            Errors =
            [
                new()
                {
                    Code = "UnknownError",
                    Description = AsaasResponse
                }
            ];
        }
    }

    public bool WasSucessfull() => StatusCode.IsSuccessStatusCode();
}
