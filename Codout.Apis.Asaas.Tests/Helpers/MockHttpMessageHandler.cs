using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Codout.Apis.Asaas.Tests.Helpers;

public class MockHttpMessageHandler : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestContent { get; private set; }

    private HttpStatusCode _statusCode = HttpStatusCode.OK;
    private string _responseContent = "{}";

    public MockHttpMessageHandler WithResponse(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _responseContent = content;
        return this;
    }

    public MockHttpMessageHandler WithOkResponse(string content)
    {
        _statusCode = HttpStatusCode.OK;
        _responseContent = content;
        return this;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        if (request.Content != null)
        {
            LastRequestContent = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        return new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_responseContent)
        };
    }
}
