using Codout.Apis.Asaas.Core.Extension;

namespace Codout.Apis.Asaas.Tests.Extensions;

public class StringExtensionsTests
{
    #region FirstCharToLower

    [Fact]
    public void FirstCharToLower_UppercaseFirst_ReturnsLowercaseFirst()
    {
        var result = "Hello".FirstCharToLower();

        Assert.Equal("hello", result);
    }

    [Fact]
    public void FirstCharToLower_AlreadyLowercase_ReturnsSame()
    {
        var result = "hello".FirstCharToLower();

        Assert.Equal("hello", result);
    }

    [Fact]
    public void FirstCharToLower_SingleUpperChar_ReturnsLower()
    {
        var result = "A".FirstCharToLower();

        Assert.Equal("a", result);
    }

    [Fact]
    public void FirstCharToLower_PascalCase_ReturnsFirstCharLowered()
    {
        var result = "PaymentLink".FirstCharToLower();

        Assert.Equal("paymentLink", result);
    }

    [Fact]
    public void FirstCharToLower_AllCaps_ReturnsFirstCharLowered()
    {
        var result = "API".FirstCharToLower();

        Assert.Equal("aPI", result);
    }

    [Fact]
    public void FirstCharToLower_WithNumbers_ReturnsFirstCharLowered()
    {
        var result = "Test123".FirstCharToLower();

        Assert.Equal("test123", result);
    }

    [Fact]
    public void FirstCharToLower_PreservesRemainingChars()
    {
        var result = "DueDateLimitDays".FirstCharToLower();

        Assert.Equal("dueDateLimitDays", result);
    }

    [Fact]
    public void FirstCharToLower_WithSpecialChars_PreservesSpecialChars()
    {
        var result = "CpfCnpj".FirstCharToLower();

        Assert.Equal("cpfCnpj", result);
    }

    #endregion
}
