using System;
using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Tests.Core;

public class RequestParametersTests
{
    #region Build

    [Fact]
    public void Build_EmptyParameters_ReturnsEmptyString()
    {
        var parameters = new RequestParameters();

        var result = parameters.Build();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Build_SingleParameter_ReturnsQueryString()
    {
        var parameters = new RequestParameters();
        parameters.Add("name", "test");

        var result = parameters.Build();

        Assert.Equal("?name=test", result);
    }

    [Fact]
    public void Build_MultipleParameters_ReturnsQueryStringWithAmpersand()
    {
        var parameters = new RequestParameters();
        parameters.Add("name", "test");
        parameters.Add("offset", "0");
        parameters.Add("limit", "10");

        var result = parameters.Build();

        Assert.StartsWith("?", result);
        Assert.Contains("name=test", result);
        Assert.Contains("offset=0", result);
        Assert.Contains("limit=10", result);
        Assert.Contains("&", result);
    }

    [Fact]
    public void Build_SpecialCharacters_EscapesValues()
    {
        var parameters = new RequestParameters();
        parameters.Add("search", "hello world");

        var result = parameters.Build();

        Assert.Contains("search=hello%20world", result);
    }

    [Fact]
    public void Build_SpecialCharactersAmpersand_EscapesValues()
    {
        var parameters = new RequestParameters();
        parameters.Add("query", "a&b");

        var result = parameters.Build();

        Assert.Contains("query=a%26b", result);
    }

    #endregion

    #region Add string overload

    [Fact]
    public void Add_StringValue_AddsKeyValue()
    {
        var parameters = new RequestParameters();

        parameters.Add("key", "value");

        Assert.Equal("value", parameters["key"]);
    }

    [Fact]
    public void Add_NullStringValue_RemovesKey()
    {
        var parameters = new RequestParameters();
        parameters.Add("key", "value");

        parameters.Add("key", (string)null!);

        Assert.Null(parameters["key"]);
        Assert.False(parameters.ContainsKey("key"));
    }

    [Fact]
    public void Add_OverwritesExistingKey()
    {
        var parameters = new RequestParameters();
        parameters.Add("key", "value1");

        parameters.Add("key", "value2");

        Assert.Equal("value2", parameters["key"]);
    }

    #endregion

    #region Add DateTime overload

    [Fact]
    public void Add_DateTimeValue_FormatsAsApiDate()
    {
        var parameters = new RequestParameters();
        var date = new DateTime(2024, 3, 15);

        parameters.Add("date", (DateTime?)date);

        Assert.Equal("2024-03-15", parameters["date"]);
    }

    [Fact]
    public void Add_NullDateTimeValue_RemovesKey()
    {
        var parameters = new RequestParameters();
        parameters.Add("date", "2024-01-01");

        parameters.Add("date", (DateTime?)null);

        Assert.False(parameters.ContainsKey("date"));
    }

    #endregion

    #region Add Enum overload

    [Fact]
    public void Add_EnumValue_AddsEnumString()
    {
        var parameters = new RequestParameters();

        parameters.Add("env", AsaasEnvironment.SANDBOX);

        Assert.Equal("SANDBOX", parameters["env"]);
    }

    #endregion

    #region Add bool overload

    [Fact]
    public void Add_BoolTrueValue_AddsStringTrue()
    {
        var parameters = new RequestParameters();

        parameters.Add("active", (bool?)true);

        Assert.Equal("True", parameters["active"]);
    }

    [Fact]
    public void Add_BoolFalseValue_AddsStringFalse()
    {
        var parameters = new RequestParameters();

        parameters.Add("active", (bool?)false);

        Assert.Equal("False", parameters["active"]);
    }

    [Fact]
    public void Add_NullBoolValue_RemovesKey()
    {
        var parameters = new RequestParameters();
        parameters.Add("active", "True");

        parameters.Add("active", (bool?)null);

        Assert.False(parameters.ContainsKey("active"));
    }

    #endregion

    #region Add decimal overload

    [Fact]
    public void Add_DecimalValue_AddsString()
    {
        var parameters = new RequestParameters();

        parameters.Add("amount", (decimal?)99.99m);

        Assert.NotNull(parameters["amount"]);
        Assert.Contains(99.99m.ToString(), parameters["amount"]);
    }

    [Fact]
    public void Add_NullDecimalValue_RemovesKey()
    {
        var parameters = new RequestParameters();
        parameters.Add("amount", "100");

        parameters.Add("amount", (decimal?)null);

        Assert.False(parameters.ContainsKey("amount"));
    }

    #endregion

    #region Indexer

    [Fact]
    public void Indexer_Get_NonExistentKey_ReturnsNull()
    {
        var parameters = new RequestParameters();

        var result = parameters["nonexistent"];

        Assert.Null(result);
    }

    [Fact]
    public void Indexer_Get_ExistingKey_ReturnsValue()
    {
        var parameters = new RequestParameters();
        parameters.Add("key", "value");

        var result = parameters["key"];

        Assert.Equal("value", result);
    }

    [Fact]
    public void Indexer_Set_OverwritesExistingValue()
    {
        var parameters = new RequestParameters();
        parameters["key"] = "value1";

        parameters["key"] = "value2";

        Assert.Equal("value2", parameters["key"]);
    }

    #endregion

    #region Get<T>

    [Fact]
    public void Get_StringValue_ReturnsString()
    {
        var parameters = new RequestParameters();
        parameters.Add("key", "hello");

        var result = parameters.Get<string>("key");

        Assert.Equal("hello", result);
    }

    [Fact]
    public void Get_MissingKey_ReturnsDefault()
    {
        var parameters = new RequestParameters();

        var result = parameters.Get<string>("missing");

        Assert.Null(result);
    }

    [Fact]
    public void Get_BoolValue_ReturnsBool()
    {
        var parameters = new RequestParameters();
        parameters.Add("active", "True");

        var result = parameters.Get<bool>("active");

        Assert.True(result);
    }

    [Fact]
    public void Get_EnumValue_ReturnsEnum()
    {
        var parameters = new RequestParameters();
        parameters.Add("env", "SANDBOX");

        var result = parameters.Get<AsaasEnvironment>("env");

        Assert.Equal(AsaasEnvironment.SANDBOX, result);
    }

    [Fact]
    public void Get_DateTimeValue_ReturnsDateTime()
    {
        var parameters = new RequestParameters();
        parameters.Add("date", "2024-03-15");

        var result = parameters.Get<DateTime>("date");

        Assert.Equal(new DateTime(2024, 3, 15), result);
    }

    [Fact]
    public void Get_NullableDateTimeValue_ReturnsNullableDateTime()
    {
        var parameters = new RequestParameters();
        parameters.Add("date", "2024-06-01");

        var result = parameters.Get<DateTime?>("date");

        Assert.NotNull(result);
        Assert.Equal(new DateTime(2024, 6, 1), result.Value);
    }

    #endregion

    #region AddRange

    [Fact]
    public void AddRange_CopiesAllEntries()
    {
        var parameters = new RequestParameters();
        parameters.Add("existing", "value0");

        var other = new RequestParameters();
        other.Add("key1", "value1");
        other.Add("key2", "value2");

        parameters.AddRange(other);

        Assert.Equal("value0", parameters["existing"]);
        Assert.Equal("value1", parameters["key1"]);
        Assert.Equal("value2", parameters["key2"]);
    }

    #endregion

    #region Add List<string> overload

    [Fact]
    public void Add_ListOfStrings_AddsLastValue()
    {
        var parameters = new RequestParameters();

        parameters.Add("tags", new System.Collections.Generic.List<string> { "tag1", "tag2", "tag3" });

        // The Add(key, List<string>) iterates and calls Add(key, value) which overwrites via the indexer
        // So only the last value remains
        Assert.Equal("tag3", parameters["tags"]);
    }

    #endregion
}
