using System;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;

namespace Codout.Apis.Asaas;

public class AsaasApi(ApiSettings apiSettings)
{
    #region Lazy
    private Lazy<CustomerManager> LazyCustomer { get; } = new(() => new CustomerManager(apiSettings), true);
    private Lazy<PaymentManager> LazyPayment { get; } = new(() => new PaymentManager(apiSettings), true);
    private Lazy<InstallmentManager> LazyInstallment { get; } = new(() => new InstallmentManager(apiSettings), true);
    private Lazy<SubscriptionManager> LazySubscription { get; } = new(() => new SubscriptionManager(apiSettings), true);
    private Lazy<FinanceManager> LazyFinance { get; } = new(() => new FinanceManager(apiSettings), true);
    private Lazy<TransferManager> LazyTransfer { get; } = new(() => new TransferManager(apiSettings), true);
    private Lazy<WalletManager> LazyWallet { get; } = new(() => new WalletManager(apiSettings), true);
    private Lazy<WebhookManager> LazyWebhook { get; } = new(() => new WebhookManager(apiSettings), true);
    private Lazy<AsaasAccountManager> LazyAsaasAccount { get; } = new(() => new AsaasAccountManager(apiSettings), true);
    private Lazy<AnticipationManager> LazyReceivableAnticipation { get; } = new(() => new AnticipationManager(apiSettings), true);
    private Lazy<MyAccountManager> LazyMyAccount { get; } = new(() => new MyAccountManager(apiSettings), true);
    private Lazy<InvoiceManager> LazyInvoice { get; } = new(() => new InvoiceManager(apiSettings), true);
    private Lazy<PaymentDunningManager> LazyPaymentDunning { get; } = new(() => new PaymentDunningManager(apiSettings), true);
    private Lazy<BillPaymentManager> LazyBillPayment { get; } = new(() => new BillPaymentManager(apiSettings), true);
    private Lazy<CreditCardManager> LazyCreditCard { get; } = new(() => new CreditCardManager(apiSettings), true);
    private Lazy<PaymentLinkManager> LazyPaymentLink { get; } = new(() => new PaymentLinkManager(apiSettings), true);
    private Lazy<NotificationManager> LazyNotification { get; } = new(() => new NotificationManager(apiSettings), true);
    private Lazy<CreditBureauReportManager> LazyCreditBureauReport { get; } = new(() => new CreditBureauReportManager(apiSettings), true);
    private Lazy<CustomerFiscalInfoManager> LazyCustomerFiscalInfo { get; } = new(() => new CustomerFiscalInfoManager(apiSettings), true);
    private Lazy<PixManager> LazyPix { get; } = new(() => new PixManager(apiSettings), true);

    #endregion

    #region Managers
    public CustomerManager Customer => LazyCustomer.Value;
    public PaymentManager Payment => LazyPayment.Value;
    public InstallmentManager Installment => LazyInstallment.Value;
    public SubscriptionManager Subscription => LazySubscription.Value;
    public FinanceManager Finance => LazyFinance.Value;
    public TransferManager Transfer => LazyTransfer.Value;
    public WalletManager Wallet => LazyWallet.Value;
    public WebhookManager Webhook => LazyWebhook.Value;
    public AsaasAccountManager AsaasAccount => LazyAsaasAccount.Value;
    public AnticipationManager ReceivableAnticipation => LazyReceivableAnticipation.Value;
    public MyAccountManager MyAccount => LazyMyAccount.Value;
    public InvoiceManager Invoice => LazyInvoice.Value;
    public PaymentDunningManager PaymentDunning => LazyPaymentDunning.Value;
    public BillPaymentManager BillPayment => LazyBillPayment.Value;
    public CreditCardManager CreditCard => LazyCreditCard.Value;
    public PaymentLinkManager PaymentLink => LazyPaymentLink.Value;
    public NotificationManager Notification => LazyNotification.Value;
    public CreditBureauReportManager CreditBureauReport => LazyCreditBureauReport.Value;
    public CustomerFiscalInfoManager CustomerFiscalInfo => LazyCustomerFiscalInfo.Value;
    public PixManager Pix => LazyPix.Value;
    #endregion
}
