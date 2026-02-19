
# Asaas SDK for .NET

<p align="center">
  <img src="asaas.png" alt="Asaas" width="120" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet" />
  <img src="https://img.shields.io/nuget/v/Asaas.Api?color=004880&logo=nuget" />
  <img src="https://img.shields.io/badge/license-MIT-green" />
  <img src="https://img.shields.io/badge/tests-400%20passing-brightgreen" />
</p>

SDK .NET **no-oficial** para integrar com a plataforma de pagamentos [Asaas](https://www.asaas.com). Cobre a **API v3** com suporte a mais de 90 endpoints.

> **Zero dependencias externas** - usa apenas `System.Text.Json` (built-in).

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

if (customerResponse.WasSucessfull())
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

    if (paymentResponse.WasSucessfull())
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

if (response.WasSucessfull())
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

O SDK expoe 20 managers via `AsaasApi`:

| Manager | Propriedade | Endpoints | Descricao |
|---------|-------------|-----------|-----------|
| **Clientes** | `asaas.Customer` | Create, Find, List, Update, Delete, Restore | Cadastro de clientes |
| **Cobrancas** | `asaas.Payment` | Create, Find, List, Update, Delete, Restore, Refund, ReceiveInCash, UndoReceivedInCash, GetBankSlipBarCode, GetPixQrCode | Boletos, Pix, Cartao |
| **Parcelamentos** | `asaas.Installment` | Find, List, Delete, Refund, ListPaymentBook | Carnezinhos/parcelas |
| **Assinaturas** | `asaas.Subscription` | Create, Find, List, Update, Delete, ListPayments, ListPaymentBook, ListInvoice, CRUD InvoiceSettings | Cobran cas recorrentes |
| **Links de Pagamento** | `asaas.PaymentLink` | Create, Find, List, Update, Delete, Restore, AddImage, ListImages, FindImage, DeleteImage, SetMainImage | Links compartilhaveis |
| **Pix** | `asaas.Pix` | ListTransactions, CancelTransaction, CreateStaticQrCode, DecodeQrCode, PayQrCode, CRUD AddressKeys | Pix completo |
| **Notas Fiscais** | `asaas.Invoice` | Schedule, Find, List, Update, Authorize, Cancel, ListMunicipalServices | NFS-e |
| **Financeiro** | `asaas.Finance` | Balance, ListTransactions, PaymentStatistics, SplitStatistics | Saldo e extrato |
| **Transferencias** | `asaas.Transfer` | Find, List, Execute (Asaas/Banco) | TED e transferencias |
| **Antecipacoes** | `asaas.ReceivableAnticipation` | Create, Simulate, Find, List, SignAgreement | Antecipacao de recebiveis |
| **Negativacoes** | `asaas.PaymentDunning` | Create, Simulate, Find, List, ListEventHistory, ListPartialPayments, ListPaymentsAvailable, ResendDocument, Cancel | Protesto e SERASA |
| **Pagto de Contas** | `asaas.BillPayment` | Create, Simulate, Find, List, Cancel | Pagamento de boletos |
| **Webhooks** | `asaas.Webhook` | CRUD Payment, Invoice e MobilePhoneRecharge | Notificacoes |
| **Notificacoes** | `asaas.Notification` | Update, BatchUpdate | Config de notificacoes |
| **Cartao de Credito** | `asaas.CreditCard` | TokenizeCreditCard | Tokenizacao |
| **Contas Asaas** | `asaas.AsaasAccount` | Create, List | Subcontas white-label |
| **Minha Conta** | `asaas.MyAccount` | Find, FindFees, FindAccountNumber, CRUD PaymentCheckoutConfig | Dados da conta |
| **Carteiras** | `asaas.Wallet` | List | Carteiras digitais |
| **Consulta SERASA** | `asaas.CreditBureauReport` | Create, Find, List | Consulta de credito |
| **Info Fiscal** | `asaas.CustomerFiscalInfo` | CreateOrUpdate, Find, ListMunicipalOptions | Config fiscal NFS-e |

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

if (link.WasSucessfull())
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

### Transferencias

```csharp
using Codout.Apis.Asaas.Models.Transfer;

// Transferencia para conta Asaas
var transfer = await asaas.Transfer.Execute(new AsaasAccountTransferRequest
{
    WalletId = "wallet_destino",
    Value = 500.00m
});

// Transferencia bancaria (TED)
var ted = await asaas.Transfer.Execute(new BankAccountTransferRequest
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
```

### Saldo e Extrato

```csharp
// Saldo atual
var balance = await asaas.Finance.Balance();

// Extrato
var transactions = await asaas.Finance.ListTransactions(0, 50);

// Estatisticas de cobran cas
var stats = await asaas.Finance.GetPaymentStatistics();
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

O projeto possui **400 testes unitarios** cobrindo todos os 20 managers, modelos, serializacao e extensions:

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
├── AsaasApi.cs                    # Facade principal (entry point)
├── Core/
│   ├── ApiSettings.cs             # Configuracao (token, ambiente, timeout)
│   ├── BaseManager.cs             # Base HTTP (GET/POST/PUT/DELETE)
│   ├── JsonSerializerConfiguration.cs  # Config System.Text.Json
│   ├── RequestParameters.cs       # Query string builder
│   ├── Response/
│   │   ├── Base/BaseResponse.cs   # StatusCode, Errors, WasSucessfull()
│   │   ├── ResponseObject<T>.cs   # Resposta de item unico
│   │   └── ResponseList<T>.cs     # Resposta paginada
│   └── Extension/                 # Helpers (DateTime, StatusCode, String)
├── Managers/                      # 20 domain managers
│   ├── CustomerManager.cs
│   ├── PaymentManager.cs
│   ├── PixManager.cs
│   └── ...
└── Models/                        # Request/Response por dominio
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
