
# Asaas SDK for .NET

<p align="center">
  <img src="asaas.png" alt="Asaas" width="120" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet" />
  <img src="https://img.shields.io/nuget/v/Asaas.Api?color=004880&logo=nuget" />
  <img src="https://img.shields.io/badge/license-MIT-green" />
  <img src="https://img.shields.io/badge/tests-492%20passing-brightgreen" />
</p>

SDK .NET **no-oficial** para integrar com a plataforma de pagamentos [Asaas](https://www.asaas.com). Cobre **100% da API v3** documentada — mais de **150 endpoints** distribuidos em **27 managers**.

> **Zero dependencias externas** - usa apenas `System.Text.Json` (built-in).

> **v3.0.0** - major release com auditoria de conformidade completa contra a documentacao oficial via MCP. Veja `CHANGELOG.md` para os breaking changes e guia de migracao 2.x -> 3.x.

---

## Instalacao

```bash
dotnet add package Asaas.Api
```

Ou via Package Manager:

```powershell
Install-Package Asaas.Api
```

## Quick Start

```csharp
using Codout.Apis.Asaas;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Customer;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Models.Common.Enums;

// 1. Configurar
var settings = new ApiSettings(
    accessToken: "SEU_ACCESS_TOKEN",
    applicationName: "MeuApp",
    asaasEnvironment: AsaasEnvironment.SANDBOX
);

var asaas = new AsaasApi(settings);

// 2. Criar cliente
var customerResponse = await asaas.Customer.Create(new CreateCustomerRequest
{
    Name = "Maria da Silva",
    CpfCnpj = "01020558075",
    Email = "maria@email.com"
});

if (customerResponse.WasSuccessful())
{
    var customer = customerResponse.Data;

    // 3. Criar cobranca
    var paymentResponse = await asaas.Payment.Create(new CreatePaymentRequest
    {
        CustomerId = customer.Id,
        BillingType = BillingType.PIX,
        Value = 150.00m,
        DueDate = DateTime.Now.AddDays(7),
        Description = "Pedido #1234"
    });

    if (paymentResponse.WasSuccessful())
    {
        // 4. Obter QR Code Pix
        var pixQr = await asaas.Payment.GetPixQrCode(paymentResponse.Data.Id);
        Console.WriteLine($"Pix payload: {pixQr.Data.Payload}");
    }
}
```

## Ambientes

| Ambiente | Enum | URL Base |
|----------|------|----------|
| **Sandbox** | `AsaasEnvironment.SANDBOX` | `https://api-sandbox.asaas.com/v3` |
| **Producao** | `AsaasEnvironment.PRODUCTION` | `https://api.asaas.com/v3` |

> Durante o periodo de testes, recomendamos usar o ambiente [Sandbox](https://sandbox.asaas.com) do Asaas.

## Tratamento de Respostas

Toda chamada retorna `ResponseObject<T>` (item unico) ou `ResponseList<T>` (lista paginada):

```csharp
// Resposta de item unico
ResponseObject<Customer> response = await asaas.Customer.Find("cus_123");

if (response.WasSuccessful())
{
    Customer customer = response.Data;
    Console.WriteLine(customer.Name);
}
else
{
    Console.WriteLine($"Status: {response.StatusCode}");
    foreach (var error in response.Errors)
    {
        Console.WriteLine($"[{error.Code}] {error.Description}");
    }
}
```

```csharp
// Resposta de lista paginada
ResponseList<Payment> list = await asaas.Payment.List(offset: 0, limit: 10);

Console.WriteLine($"Total: {list.TotalCount}");
Console.WriteLine($"Tem mais? {list.HasMore}");

foreach (var payment in list.Data)
{
    Console.WriteLine($"{payment.Id} - R$ {payment.Value}");
}
```

## Filtros em Listagens

A maioria dos metodos `List` aceita um objeto de filtro opcional:

```csharp
var payments = await asaas.Payment.List(0, 20, new PaymentListFilter
{
    Customer = "cus_123",
    BillingType = BillingType.PIX,
    Status = PaymentStatus.PENDING
});
```

```csharp
var customers = await asaas.Customer.List(0, 50, new CustomerListFilter
{
    Name = "Maria",
    CpfCnpj = "01020558075"
});
```

## Modulos Disponiveis

O SDK expoe 27 managers via `AsaasApi`:

| Manager | Propriedade | Descricao |
|---------|-------------|-----------|
| **Clientes** | `asaas.Customer` | CRUD de clientes |
| **Cobrancas** | `asaas.Payment` | Boletos, Pix, Cartao, documentos, splits, refunds, billing/viewing info, simulate, limits, etc |
| **Parcelamentos** | `asaas.Installment` | Create, CreateWithCreditCard, Find, List, Delete, Refund, ListPayments, CancelPendingPayments, UpdateSplits, ListPaymentBook |
| **Assinaturas** | `asaas.Subscription` | CRUD + UpdateCreditCard + InvoiceSettings + ListPayments |
| **Links de Pagamento** | `asaas.PaymentLink` | CRUD + imagens (add, list, find, delete, setMain) |
| **Pix** | `asaas.Pix` | CRUD AddressKeys, QrCodes (estatico, decode, pay, delete), Transactions (list, find, cancel), tokenBucket |
| **Pix Automatico** | `asaas.PixAutomatic` | Autorizacoes recorrentes (Pix Bacen) e payment instructions |
| **Pix Recorrente** | `asaas.PixRecurring` | Recorrencias parceladas e itens |
| **Webhooks** | `asaas.Webhook` | CRUD generico em `/v3/webhooks/{id}` com ~100 eventos disponiveis |
| **Notas Fiscais** | `asaas.Invoice` | Schedule, Find, List, Update, Authorize, Cancel |
| **Info Fiscal** | `asaas.FiscalInfo` | CreateOrUpdate, Find, ListMunicipalOptions, ListServices |
| **Financeiro** | `asaas.Finance` | GetBalance, ListTransactions, GetPaymentStatistics, GetSplitStatistics |
| **Transferencias** | `asaas.Transfer` | TransferToAsaasAccount, TransferToBankAccount, Find, List, Cancel |
| **Antecipacoes** | `asaas.Anticipation` | Create, Simulate, Find, List, Cancel, GetLimits, GetAutomaticConfiguration, UpdateAutomaticConfiguration |
| **Negativacoes** | `asaas.PaymentDunning` | Create, Simulate, Find, List, ListEventHistory, ListPartialPayments, ListPaymentsAvailable, ResendDocument, Cancel |
| **Pagto de Contas** | `asaas.BillPayment` | Create, Simulate, Find, List, Cancel |
| **Recarga Celular** | `asaas.MobilePhoneRecharge` | Create, Find, List, Cancel, GetProvider |
| **Notificacoes** | `asaas.Notification` | Update, BatchUpdate |
| **Cartao de Credito** | `asaas.CreditCard` | TokenizeCreditCard + PreAuthorization config |
| **Contas Asaas** | `asaas.AsaasAccount` | Create, Find, List, ResendActivationLink, CRUD AccessTokens |
| **Minha Conta** | `asaas.MyAccount` | GetCommercialInfo, UpdateCommercialInfo, GetStatus, DeleteWhiteLabelAccount, GetFees, GetAccountNumber, PaymentCheckoutConfig, Documents |
| **Carteiras** | `asaas.Wallet` | List |
| **Consulta SERASA** | `asaas.CreditBureauReport` | Create, Find, List |
| **Chargebacks** | `asaas.Chargeback` | List, FindByPayment, CreateDispute |
| **Escrow** | `asaas.Escrow` | SaveSubaccountConfig, GetSubaccountConfig, SaveDefaultConfig, GetDefaultConfig, FinishPaymentEscrow, GetPaymentEscrow |
| **Checkout** | `asaas.Checkout` | Create, Cancel |
| **Sandbox (testes)** | `asaas.Sandbox` | ApproveAccount, ConfirmPayment, ForceOverdue (so funciona em `AsaasEnvironment.SANDBOX`) |

## Exemplos por Modulo

### Assinaturas (Recorrencia)

```csharp
using Codout.Apis.Asaas.Models.Subscription;
using Codout.Apis.Asaas.Models.Subscription.Enums;

var subscription = await asaas.Subscription.Create(new CreateSubscriptionRequest
{
    CustomerId = "cus_123",
    BillingType = BillingType.CREDIT_CARD,
    Value = 99.90m,
    NextDueDate = DateTime.Now.AddDays(30),
    Cycle = Cycle.MONTHLY,
    Description = "Plano Premium",
    CreditCard = new CreditCardRequest
    {
        HolderName = "Maria da Silva",
        Number = "4111111111111111",
        ExpiryMonth = "12",
        ExpiryYear = "2028",
        Ccv = "123"
    },
    CreditCardHolderInfo = new CreditCardHolderInfoRequest
    {
        Name = "Maria da Silva",
        CpfCnpj = "01020558075",
        Email = "maria@email.com",
        PostalCode = "01001000",
        AddressNumber = "100",
        Phone = "11999999999"
    }
});

// Listar cobran cas de uma assinatura
var payments = await asaas.Subscription.ListPayments("sub_123", offset: 0, limit: 20);
```

### Links de Pagamento

```csharp
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;

var link = await asaas.PaymentLink.Create(new CreatePaymentLinkRequest
{
    Name = "Produto XYZ",
    Description = "Descricao do produto",
    Value = 199.90m,
    BillingType = BillingType.UNDEFINED,
    ChargeType = ChargeType.DETACHED,
    DueDateLimitDays = 10
});

if (link.WasSuccessful())
{
    Console.WriteLine($"Link: {link.Data.Url}");
}
```

### Pix

```csharp
using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;

// Criar QR Code estatico
var qrCode = await asaas.Pix.CreateStaticQrCode(new CreatePixStaticQrCodeRequest
{
    AddressKey = "sua-chave-pix",
    Description = "Doacao",
    Value = 50.00m
});

// Listar chaves Pix
var keys = await asaas.Pix.ListAddressKeys(offset: 0, limit: 10);

// Criar chave aleatoria
var newKey = await asaas.Pix.CreateAddressKey(new CreatePixAddressKeyRequest
{
    Type = PixAddressKeyType.EVP
});
```

### Webhooks

```csharp
using Codout.Apis.Asaas.Models.Webhook;
using Codout.Apis.Asaas.Models.Webhook.Enums;

var webhook = await asaas.Webhook.Create(new CreateWebhookRequest
{
    Name = "Notificacoes de pagamento",
    Url = "https://meusite.com/webhook/asaas",
    Email = "ops@meusite.com",
    Enabled = true,
    Interrupted = false,
    ApiVersion = 3,
    AuthToken = "whsec_min_32_caracteres_para_assinar_callbacks",
    SendType = WebhookSendType.SEQUENTIALLY,
    Events =
    [
        WebhookEvent.PAYMENT_CONFIRMED,
        WebhookEvent.PAYMENT_RECEIVED,
        WebhookEvent.PAYMENT_OVERDUE
    ]
});
```

### Transferencias

```csharp
using Codout.Apis.Asaas.Models.Transfer;

// Transferencia para conta Asaas (POST /v3/transfers/)
var transfer = await asaas.Transfer.TransferToAsaasAccount(new AsaasAccountTransferRequest
{
    WalletId = "wallet_destino",
    Value = 500.00m
});

// Transferencia bancaria (TED / Pix para outra instituicao - POST /v3/transfers)
var ted = await asaas.Transfer.TransferToBankAccount(new BankAccountTransferRequest
{
    Value = 1000.00m,
    BankAccount = new BankAccount
    {
        Bank = new Bank { Code = "341" },
        AccountName = "Empresa LTDA",
        OwnerName = "Empresa LTDA",
        CpfCnpj = "12345678000100",
        Agency = "1234",
        Account = "56789",
        AccountDigit = "0",
        BankAccountType = BankAccountType.CONTA_CORRENTE
    }
});

// Cancelar transferencia
await asaas.Transfer.Cancel("trans_123");
```

### Notas Fiscais (NFS-e)

```csharp
using Codout.Apis.Asaas.Models.Invoice;

var invoice = await asaas.Invoice.Schedule(new CreateInvoiceRequest
{
    PaymentId = "pay_123",
    ServiceDescription = "Servico de consultoria",
    Observations = "Referente ao mes de janeiro"
});

// Autorizar emissao
await asaas.Invoice.Authorize("inv_123");

// Listar codigos de servico do municipio (FiscalInfo, nao Invoice)
var services = await asaas.FiscalInfo.ListServices("consultoria");
```

### Saldo e Extrato

```csharp
// Saldo atual (retorna { balance: number } - acesse via .Value)
var balance = await asaas.Finance.GetBalance();
Console.WriteLine($"Saldo: R$ {balance.Data.Value}");

// Extrato
var transactions = await asaas.Finance.ListTransactions(0, 50);

// Estatisticas de cobran cas
var stats = await asaas.Finance.GetPaymentStatistics();
```

### Sandbox (somente em testes)

```csharp
// Em AsaasEnvironment.SANDBOX, voce pode acelerar fluxos de teste:
await asaas.Sandbox.ApproveAccount();           // aprova conta sandbox
await asaas.Sandbox.ConfirmPayment("pay_123");  // confirma pagamento
await asaas.Sandbox.ForceOverdue("pay_123");    // forca vencimento

// Em PRODUCTION, todos lancam InvalidOperationException antes de fazer HTTP.
```

## Configuracao Avancada

### Timeout Customizado

```csharp
var settings = new ApiSettings("TOKEN", "App", AsaasEnvironment.PRODUCTION);
settings.TimeOut = TimeSpan.FromSeconds(60);
```

## Desenvolvimento

### Pre-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Build

```bash
dotnet build Codout.Apis.Asaas/Codout.Apis.Asaas.csproj
```

### Testes

O projeto possui **492 testes unitarios** cobrindo todos os 27 managers, modelos, serializacao e extensions:

```bash
# Rodar todos os testes
dotnet test Codout.Apis.Asaas.Tests/

# Rodar testes de um manager especifico
dotnet test Codout.Apis.Asaas.Tests/ --filter "FullyQualifiedName~CustomerManagerTests"

# Com output detalhado
dotnet test Codout.Apis.Asaas.Tests/ --verbosity normal
```

### Gerar pacote NuGet

```bash
dotnet pack Codout.Apis.Asaas/ -c Release
```

## Arquitetura

```
Codout.Apis.Asaas/
├── AsaasApi.cs                    # Facade principal (entry point) com 27 managers
├── Core/
│   ├── ApiSettings.cs             # Configuracao (token, ambiente, timeout)
│   ├── BaseManager.cs             # Base HTTP (GET/POST/PUT/DELETE) com SocketsHttpHandler compartilhado
│   ├── JsonSerializerConfiguration.cs  # Config System.Text.Json (camelCase, SafeEnumConverterFactory, FlexibleDateTimeConverter)
│   ├── RequestParameters.cs       # Query string builder
│   ├── Response/
│   │   ├── Base/BaseResponse.cs   # StatusCode, Errors, WasSuccessful()
│   │   ├── ResponseObject<T>.cs   # Resposta de item unico
│   │   └── ResponseList<T>.cs     # Resposta paginada
│   └── Extension/                 # Helpers (DateTime, StatusCode, String)
├── Managers/                      # 27 domain managers
│   ├── CustomerManager.cs
│   ├── PaymentManager.cs
│   ├── PixManager.cs
│   ├── PixAutomaticManager.cs
│   ├── PixRecurringManager.cs
│   ├── ChargebackManager.cs
│   ├── EscrowManager.cs
│   └── ...
└── Models/                        # Request/Response por dominio (1 classe por arquivo)
    ├── Customer/
    ├── Payment/
    ├── Pix/
    └── ...
```

## Documentacao da API Asaas

- [Documentacao Oficial](https://docs.asaas.com)
- [Ambiente Sandbox](https://sandbox.asaas.com)
- [Painel de Producao](https://www.asaas.com)

## Contribuindo

Contribuicoes sao bem-vindas! Sinta-se livre para:

1. Fazer fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas alteracoes (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abrir um Pull Request

## Licenca

Este projeto e licenciado sob a licenca MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## Autor

**Clovis Coli Jr** - [Codout](https://codout.com)

---

> **Aviso:** Este e um SDK no-oficial. Nao possui vinculo com a Asaas. Para suporte oficial da API, consulte a [documentacao do Asaas](https://docs.asaas.com).
