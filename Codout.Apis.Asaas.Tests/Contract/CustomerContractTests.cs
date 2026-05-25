using System;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Customer;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para CustomerManager (B-25).
/// Schemas verificados via MCP em 2026-05-24:
/// - POST /v3/customers (request: name+cpfCnpj required)
/// - GET /v3/customers (envelope padrao + 5 filtros)
/// - GET /v3/customers/{id}
/// - PUT /v3/customers/{id}
/// - DELETE /v3/customers/{id} (BaseDeleted shape)
/// - POST /v3/customers/{id}/restore (body vazio)
/// - GET /v3/customers/{id}/notifications (envelope padrao)
/// </summary>
public class CustomerContractTests
{
    [Fact]
    public void CreateRequest_OnlyRequiredFieldsSerialized()
    {
        var request = new CreateCustomerRequest { Name = "John Doe", CpfCnpj = "24971563792" };
        JsonContractAssert.SerializesWithKeys(request, "name", "cpfCnpj");
        // notificationDisabled e bool? -> nao deve aparecer no JSON quando nao definido
        JsonContractAssert.DoesNotSerializeKey(request, "notificationDisabled");
    }

    [Fact]
    public void CreateRequest_FullPayloadMatchesSchema()
    {
        var request = new CreateCustomerRequest
        {
            Name = "John Doe",
            CpfCnpj = "24971563792",
            Email = "john.doe@asaas.com.br",
            MobilePhone = "4799376637",
            Address = "Av. Paulista",
            AddressNumber = "150",
            Complement = "Sala 201",
            Province = "Centro",
            PostalCode = "01310-000",
            ExternalReference = "12987382",
            NotificationDisabled = false,
            AdditionalEmails = "john.doe@asaas.com",
            MunicipalInscription = "46683695908",
            StateInscription = "646681195275",
            Observations = "great payer",
            GroupName = "vip",
            Company = "Acme",
            ForeignCustomer = false
        };

        JsonContractAssert.SerializesWithKeys(request,
            "name", "cpfCnpj", "email", "mobilePhone", "address", "addressNumber",
            "complement", "province", "postalCode", "externalReference",
            "notificationDisabled", "additionalEmails", "municipalInscription",
            "stateInscription", "observations", "groupName", "company", "foreignCustomer");
    }

    [Fact]
    public void UpdateRequest_NotificationDisabledIsNullable()
    {
        // B-25e regression: era bool nao-nulavel, forcando false em todo Update
        // que nao setasse explicitamente o campo.
        var request = new UpdateCustomerRequest { Name = "x" };
        JsonContractAssert.DoesNotSerializeKey(request, "notificationDisabled");
    }

    [Fact]
    public void CustomerResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Customer/customer-response.json");

        var result = JsonContractAssert.DeserializeFixture<Customer>(json);

        Assert.Equal("cus_000005401844", result.Id);
        Assert.Equal("customer", result.Object);
        Assert.Equal(new DateTime(2024, 7, 12), result.DateCreated);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john.doe@asaas.com.br", result.Email);
        Assert.Equal(12565L, result.CityId);
        Assert.Equal("São Paulo", result.CityName);
        Assert.Equal("SP", result.State);
        Assert.Equal("Brasil", result.Country);
        Assert.Equal("24971563792", result.CpfCnpj);
        Assert.Equal(PersonType.FISICA, result.PersonType);
        Assert.False(result.Deleted);
        Assert.False(result.NotificationDisabled);
        Assert.False(result.ForeignCustomer);
    }

    [Fact]
    public void CustomerResponse_DateCreatedIsNullable()
    {
        // B-25a regression: era DateTime non-nullable; deserializacao falhava
        // quando o JSON omitia dateCreated.
        var json = "{\"id\":\"cus_x\"}";
        var result = JsonContractAssert.DeserializeFixture<Customer>(json);
        Assert.Null(result.DateCreated);
    }

    [Fact]
    public void CustomersList_UsesStandardEnvelopeWithPagination()
    {
        var json = FixtureLoader.Load("Customer/customers-list-response.json");
        var response = new ResponseList<Customer>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.False(response.HasMore);
        Assert.Equal(10, response.Limit);
        Assert.Equal(0, response.Offset);
        Assert.Single(response.Data);
        Assert.Equal("cus_000005401844", response.Data[0].Id);
    }

    [Fact]
    public void PersonType_BothValuesDeserialize()
    {
        foreach (var person in new[] { "FISICA", "JURIDICA" })
        {
            var json = $"{{\"id\":\"x\",\"personType\":\"{person}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Customer>(json);
            Assert.NotNull(result.PersonType);
            Assert.Equal(person, result.PersonType.ToString());
        }
    }

    [Fact]
    public void ListFilter_SerializesAllFiveFieldsWithCorrectNames()
    {
        var filter = new CustomerListFilter
        {
            Name = "John Doe",
            Email = "john@example.com",
            CpfCnpj = "24971563792",
            GroupName = "vip",
            ExternalReference = "ext_42"
        };

        JsonContractAssert.QueryParamEquals(filter, "name", "John Doe");
        JsonContractAssert.QueryParamEquals(filter, "email", "john@example.com");
        JsonContractAssert.QueryParamEquals(filter, "cpfCnpj", "24971563792");
        JsonContractAssert.QueryParamEquals(filter, "groupName", "vip");
        JsonContractAssert.QueryParamEquals(filter, "externalReference", "ext_42");
    }
}
