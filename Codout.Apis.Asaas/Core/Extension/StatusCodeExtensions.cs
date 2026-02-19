using System.Net;

namespace Codout.Apis.Asaas.Core.Extension;

public static class StatusCodeExtensions
{
    public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
    {
        return (int)statusCode is >= 200 and <= 299;
    }
}