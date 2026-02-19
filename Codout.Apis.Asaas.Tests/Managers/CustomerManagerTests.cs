using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Customer;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class CustomerManagerTests : ManagerTestBase<CustomerManager>
{
    protected override CustomerManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableCustomerManager(settings, handler);

    #region Create

    [Fact]
    public async Task Create_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"name\":\"Test Customer\"}");
        var request = new CreateCustomerRequest { Name = "Test Customer" };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/customers");
    }

    [Fact]
    public async Task Create_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"name\":\"Test Customer\",\"email\":\"test@example.com\",\"cpfCnpj\":\"12345678901\"}");
        var request = new CreateCustomerRequest { Name = "Test Customer", Email = "test@example.com", CpfCnpj = "12345678901" };

        var result = await Manager.Create(request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("cus_123", result.Data.Id);
        Assert.Equal("Test Customer", result.Data.Name);
        Assert.Equal("test@example.com", result.Data.Email);
        Assert.Equal("12345678901", result.Data.CpfCnpj);
    }

    [Fact]
    public async Task Create_WhenApiReturnsError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);
        var request = new CreateCustomerRequest { Name = "Test" };

        var result = await Manager.Create(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region Find

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"cus_456\",\"name\":\"Found Customer\"}");

        var result = await Manager.Find("cus_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/customers/cus_456");
    }

    [Fact]
    public async Task Find_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"cus_456\",\"name\":\"Found Customer\",\"deleted\":false}");

        var result = await Manager.Find("cus_456");

        Assert.True(result.WasSucessfull());
        Assert.Equal("cus_456", result.Data.Id);
        Assert.Equal("Found Customer", result.Data.Name);
        Assert.False(result.Data.Deleted);
    }

    #endregion

    #region List

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Customer>("[{\"id\":\"cus_1\",\"name\":\"Customer 1\"}]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/customers");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_DeserializesListResponseCorrectly()
    {
        SetupListResponse<Customer>("[{\"id\":\"cus_1\",\"name\":\"Customer 1\"},{\"id\":\"cus_2\",\"name\":\"Customer 2\"}]", totalCount: 2);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("cus_1", result.Data[0].Id);
        Assert.Equal("cus_2", result.Data[1].Id);
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParametersInUrl()
    {
        SetupListResponse<Customer>("[{\"id\":\"cus_1\",\"name\":\"John\"}]");
        var filter = new CustomerListFilter { Name = "John", Email = "john@example.com" };

        var result = await Manager.List(0, 10, filter);

        AssertRequestUrlContains("name=John");
        AssertRequestUrlContains("email=john%40example.com");
    }

    [Fact]
    public async Task List_WithPagination_IncludesOffsetAndLimit()
    {
        SetupListResponse<Customer>("[]", totalCount: 0, limit: 5, offset: 10);

        var result = await Manager.List(10, 5);

        AssertRequestUrlContains("offset=10");
        AssertRequestUrlContains("limit=5");
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"name\":\"Updated Name\"}");
        var request = new UpdateCustomerRequest { Name = "Updated Name" };

        var result = await Manager.Update("cus_123", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/customers/cus_123");
    }

    [Fact]
    public async Task Update_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"name\":\"Updated Name\",\"email\":\"updated@example.com\"}");
        var request = new UpdateCustomerRequest { Name = "Updated Name", Email = "updated@example.com" };

        var result = await Manager.Update("cus_123", request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("cus_123", result.Data.Id);
        Assert.Equal("Updated Name", result.Data.Name);
        Assert.Equal("updated@example.com", result.Data.Email);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_SendsDeleteToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"deleted\":true}");

        var result = await Manager.Delete("cus_123");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/customers/cus_123");
    }

    [Fact]
    public async Task Delete_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"deleted\":true}");

        var result = await Manager.Delete("cus_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("cus_123", result.Data.Id);
        Assert.True(result.Data.Deleted);
    }

    #endregion

    #region Restore

    [Fact]
    public async Task Restore_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"name\":\"Restored Customer\",\"deleted\":false}");

        var result = await Manager.Restore("cus_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/customers/cus_123/restore");
    }

    [Fact]
    public async Task Restore_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"cus_123\",\"name\":\"Restored Customer\",\"deleted\":false}");

        var result = await Manager.Restore("cus_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("cus_123", result.Data.Id);
        Assert.False(result.Data.Deleted);
    }

    #endregion
}
