using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Tests.Helpers;

public abstract class ManagerTestBase<TManager> where TManager : BaseManager
{
    protected MockHttpMessageHandler Handler { get; }
    protected TManager Manager { get; }
    protected ApiSettings Settings { get; }

    protected ManagerTestBase()
    {
        Handler = new MockHttpMessageHandler();
        Settings = new ApiSettings("test_api_key", "TestApp", AsaasEnvironment.SANDBOX);
        Manager = CreateManager(Settings, Handler);
    }

    protected abstract TManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler);

    protected void SetupOkResponse(string json)
    {
        Handler.WithOkResponse(json);
    }

    protected void SetupListResponse<T>(string dataJson, int totalCount = 1, int limit = 10, int offset = 0, bool hasMore = false)
    {
        var json = $"{{\"hasMore\":{(hasMore ? "true" : "false")},\"totalCount\":{totalCount},\"limit\":{limit},\"offset\":{offset},\"data\":{dataJson}}}";
        Handler.WithOkResponse(json);
    }

    protected void SetupErrorResponse(HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Handler.WithResponse(statusCode, "{\"errors\":[{\"code\":\"invalid\",\"description\":\"Test error\"}]}");
    }

    protected void AssertRequestMethod(HttpMethod expectedMethod)
    {
        Assert.NotNull(Handler.LastRequest);
        Assert.Equal(expectedMethod, Handler.LastRequest.Method);
    }

    protected void AssertRequestUrl(string expectedPath)
    {
        Assert.NotNull(Handler.LastRequest);
        var actualPath = Handler.LastRequest.RequestUri?.PathAndQuery;
        Assert.Equal(expectedPath, actualPath);
    }

    protected void AssertRequestUrlContains(string expectedSubstring)
    {
        Assert.NotNull(Handler.LastRequest);
        var actualPath = Handler.LastRequest.RequestUri?.PathAndQuery ?? "";
        Assert.Contains(expectedSubstring, actualPath);
    }
}
