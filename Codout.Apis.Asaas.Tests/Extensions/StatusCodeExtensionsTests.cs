using System.Net;
using Codout.Apis.Asaas.Core.Extension;

namespace Codout.Apis.Asaas.Tests.Extensions;

public class StatusCodeExtensionsTests
{
    #region IsSuccessStatusCode - Success Codes

    [Theory]
    [InlineData(HttpStatusCode.OK)]                   // 200
    [InlineData(HttpStatusCode.Created)]              // 201
    [InlineData(HttpStatusCode.Accepted)]             // 202
    [InlineData(HttpStatusCode.NoContent)]            // 204
    public void IsSuccessStatusCode_2xxCodes_ReturnsTrue(HttpStatusCode statusCode)
    {
        Assert.True(statusCode.IsSuccessStatusCode());
    }

    #endregion

    #region IsSuccessStatusCode - Boundary Cases

    [Fact]
    public void IsSuccessStatusCode_200_ReturnsTrue()
    {
        Assert.True(HttpStatusCode.OK.IsSuccessStatusCode());
    }

    [Fact]
    public void IsSuccessStatusCode_299_ReturnsTrue()
    {
        // 299 is not a standard code but should return true per the 200-299 range logic
        Assert.True(((HttpStatusCode)299).IsSuccessStatusCode());
    }

    [Fact]
    public void IsSuccessStatusCode_199_ReturnsFalse()
    {
        Assert.False(((HttpStatusCode)199).IsSuccessStatusCode());
    }

    [Fact]
    public void IsSuccessStatusCode_300_ReturnsFalse()
    {
        Assert.False(((HttpStatusCode)300).IsSuccessStatusCode());
    }

    #endregion

    #region IsSuccessStatusCode - Client Error Codes

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]           // 400
    [InlineData(HttpStatusCode.Unauthorized)]         // 401
    [InlineData(HttpStatusCode.Forbidden)]            // 403
    [InlineData(HttpStatusCode.NotFound)]             // 404
    [InlineData(HttpStatusCode.MethodNotAllowed)]     // 405
    [InlineData(HttpStatusCode.Conflict)]             // 409
    [InlineData(HttpStatusCode.UnprocessableEntity)]  // 422
    [InlineData(HttpStatusCode.TooManyRequests)]      // 429
    public void IsSuccessStatusCode_4xxCodes_ReturnsFalse(HttpStatusCode statusCode)
    {
        Assert.False(statusCode.IsSuccessStatusCode());
    }

    #endregion

    #region IsSuccessStatusCode - Server Error Codes

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]  // 500
    [InlineData(HttpStatusCode.BadGateway)]           // 502
    [InlineData(HttpStatusCode.ServiceUnavailable)]   // 503
    [InlineData(HttpStatusCode.GatewayTimeout)]       // 504
    public void IsSuccessStatusCode_5xxCodes_ReturnsFalse(HttpStatusCode statusCode)
    {
        Assert.False(statusCode.IsSuccessStatusCode());
    }

    #endregion

    #region IsSuccessStatusCode - Informational Codes

    [Theory]
    [InlineData(HttpStatusCode.Continue)]             // 100
    [InlineData(HttpStatusCode.SwitchingProtocols)]   // 101
    public void IsSuccessStatusCode_1xxCodes_ReturnsFalse(HttpStatusCode statusCode)
    {
        Assert.False(statusCode.IsSuccessStatusCode());
    }

    #endregion

    #region IsSuccessStatusCode - Redirect Codes

    [Theory]
    [InlineData(HttpStatusCode.Moved)]                // 301
    [InlineData(HttpStatusCode.Found)]                // 302
    [InlineData(HttpStatusCode.NotModified)]          // 304
    public void IsSuccessStatusCode_3xxCodes_ReturnsFalse(HttpStatusCode statusCode)
    {
        Assert.False(statusCode.IsSuccessStatusCode());
    }

    #endregion
}
