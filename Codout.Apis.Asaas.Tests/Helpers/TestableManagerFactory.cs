using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;

namespace Codout.Apis.Asaas.Tests.Helpers;

// Testable subclasses that override BuildHttpClient to return a mocked HttpClient

public class TestableCustomerManager(ApiSettings settings, HttpMessageHandler handler) : CustomerManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestablePaymentManager(ApiSettings settings, HttpMessageHandler handler) : PaymentManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableInstallmentManager(ApiSettings settings, HttpMessageHandler handler) : InstallmentManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableSubscriptionManager(ApiSettings settings, HttpMessageHandler handler) : SubscriptionManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableFinanceManager(ApiSettings settings, HttpMessageHandler handler) : FinanceManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableTransferManager(ApiSettings settings, HttpMessageHandler handler) : TransferManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableWalletManager(ApiSettings settings, HttpMessageHandler handler) : WalletManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableWebhookManager(ApiSettings settings, HttpMessageHandler handler) : WebhookManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableAsaasAccountManager(ApiSettings settings, HttpMessageHandler handler) : AsaasAccountManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableAnticipationManager(ApiSettings settings, HttpMessageHandler handler) : AnticipationManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableMyAccountManager(ApiSettings settings, HttpMessageHandler handler) : MyAccountManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableInvoiceManager(ApiSettings settings, HttpMessageHandler handler) : InvoiceManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestablePaymentDunningManager(ApiSettings settings, HttpMessageHandler handler) : PaymentDunningManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableBillPaymentManager(ApiSettings settings, HttpMessageHandler handler) : BillPaymentManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableCreditCardManager(ApiSettings settings, HttpMessageHandler handler) : CreditCardManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestablePaymentLinkManager(ApiSettings settings, HttpMessageHandler handler) : PaymentLinkManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableNotificationManager(ApiSettings settings, HttpMessageHandler handler) : NotificationManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableCreditBureauReportManager(ApiSettings settings, HttpMessageHandler handler) : CreditBureauReportManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestableCustomerFiscalInfoManager(ApiSettings settings, HttpMessageHandler handler) : CustomerFiscalInfoManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}

public class TestablePixManager(ApiSettings settings, HttpMessageHandler handler) : PixManager(settings)
{
    protected override HttpClient BuildHttpClient()
    {
        var client = new HttpClient(handler);
        client.BaseAddress = new System.Uri("https://api-sandbox.asaas.com");
        return client;
    }
}
