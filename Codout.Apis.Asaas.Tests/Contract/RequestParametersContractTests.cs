using System;
using System.Globalization;
using System.Threading;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests sistemicos para a serializacao de query params em
/// RequestParameters. Detecta regressoes em casing, cultura, formato de
/// data — tudo que ja causou bug no SDK (B-15) ou poderia causar.
/// </summary>
public class RequestParametersContractTests
{
    [Fact]
    public void Bool_True_SerializesAsLowercaseTrue()
    {
        var parameters = new RequestParameters();
        parameters.Add("enabled", (bool?)true);

        JsonContractAssert.QueryParamEquals(parameters, "enabled", "true");
    }

    [Fact]
    public void Bool_False_SerializesAsLowercaseFalse()
    {
        var parameters = new RequestParameters();
        parameters.Add("enabled", (bool?)false);

        JsonContractAssert.QueryParamEquals(parameters, "enabled", "false");
    }

    [Fact]
    public void Bool_Null_RemovesKey()
    {
        var parameters = new RequestParameters();
        parameters.Add("enabled", (bool?)null);

        Assert.False(parameters.ContainsKey("enabled"));
    }

    [Theory]
    [InlineData("pt-BR")]
    [InlineData("es-ES")]
    [InlineData("de-DE")]
    [InlineData("en-US")]
    public void Decimal_SerializesWithDotInAllCultures(string cultureName)
    {
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            var parameters = new RequestParameters();
            parameters.Add("value", (decimal?)12.5m);

            // Em pt-BR/es-ES/de-DE, decimal.ToString() retorna "12,5" (virgula)
            // sem CultureInfo.InvariantCulture. API Asaas (JSON) exige ponto.
            JsonContractAssert.QueryParamEquals(parameters, "value", "12.5");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
        }
    }

    [Theory]
    [InlineData("pt-BR")]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    public void DateTime_SerializesAsIsoYyyyMmDdInAllCultures(string cultureName)
    {
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            var parameters = new RequestParameters();
            parameters.Add("paymentDate", new DateTime(2026, 3, 15));

            JsonContractAssert.QueryParamEquals(parameters, "paymentDate", "2026-03-15");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
        }
    }

    [Fact]
    public void Enum_SerializesAsUppercaseAsaasName()
    {
        var parameters = new RequestParameters();
        parameters.Add("billingType", BillingType.PIX);

        // Asaas enums sao em UPPERCASE e BillingType (C#) usa os mesmos
        // identificadores — Enum.ToString() retorna "PIX". Se um futuro
        // enum diferir entre C# e API, este teste falha e nos forca a
        // adicionar um conversor explicito.
        JsonContractAssert.QueryParamEquals(parameters, "billingType", "PIX");
    }

    [Fact]
    public void Build_BuildsCorrectQueryStringWithEscaping()
    {
        var parameters = new RequestParameters();
        parameters.Add("name", "John & Maria");
        parameters.Add("enabled", (bool?)true);

        var query = parameters.Build();

        Assert.StartsWith("?", query);
        Assert.Contains("name=John%20%26%20Maria", query);
        Assert.Contains("enabled=true", query);
    }

    [Fact]
    public void Build_NoParams_ReturnsEmpty()
    {
        var parameters = new RequestParameters();
        Assert.Equal(string.Empty, parameters.Build());
    }

    [Fact]
    public void Add_OverwritesExistingKey()
    {
        var parameters = new RequestParameters();
        parameters.Add("enabled", (bool?)true);
        parameters.Add("enabled", (bool?)false);

        JsonContractAssert.QueryParamEquals(parameters, "enabled", "false");
    }
}
