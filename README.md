<div align="center">

<img src="asaas.png" alt="Asaas" width="120" />

# Asaas SDK for .NET

**Cliente .NET tipado, sem dependências externas, para a API v3 do [Asaas](https://www.asaas.com).**

[![NuGet Version](https://img.shields.io/nuget/v/Asaas.Api?color=004880&logo=nuget&label=NuGet)](https://www.nuget.org/packages/Asaas.Api)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Asaas.Api?color=004880&logo=nuget&label=Downloads)](https://www.nuget.org/packages/Asaas.Api)
[![CI](https://github.com/Codout/Codout.Apis.Asaas/actions/workflows/ci.yml/badge.svg)](https://github.com/Codout/Codout.Apis.Asaas/actions/workflows/ci.yml)
[![Integration (sandbox)](https://github.com/Codout/Codout.Apis.Asaas/actions/workflows/integration-sandbox.yml/badge.svg)](https://github.com/Codout/Codout.Apis.Asaas/actions/workflows/integration-sandbox.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download)
[![Schema-first](https://img.shields.io/badge/Schema--first%20audit-27%2F27-blue)](CONFORMANCE.md)

[Instalação](#instalação) ·
[Quick Start](#quick-start) ·
[Managers](#managers-disponíveis) ·
[Conformidade](CONFORMANCE.md) ·
[Changelog](CHANGELOG.md) ·
[Contribuir](#contribuindo)

</div>

---

## Sumário

- [Visão Geral](#visão-geral)
- [Destaques](#destaques)
- [Requisitos](#requisitos)
- [Instalação](#instalação)
- [Quick Start](#quick-start)
- [Conceitos Centrais](#conceitos-centrais)
  - [Ambientes](#ambientes)
  - [Responses tipadas](#responses-tipadas)
  - [Filtros e paginação](#filtros-e-paginação)
  - [Tratamento de erros](#tratamento-de-erros)
  - [Cultura e serialização](#cultura-e-serialização)
- [Managers Disponíveis](#managers-disponíveis)
- [Exemplos por Domínio](#exemplos-por-domínio)
  - [Cobranças (Pix / Boleto / Cartão)](#cobranças-pix--boleto--cartão)
  - [Assinaturas](#assinaturas)
  - [Links de pagamento](#links-de-pagamento)
  - [Pix](#pix)
  - [Webhooks](#webhooks)
  - [Transferências](#transferências)
  - [Notas fiscais (NFS-e)](#notas-fiscais-nfs-e)
  - [Saldo e estatísticas](#saldo-e-estatísticas)
  - [Sandbox (apenas testes)](#sandbox-apenas-testes)
- [Configuração Avançada](#configuração-avançada)
- [Qualidade e Conformidade](#qualidade-e-conformidade)
- [Testes](#testes)
- [Arquitetura](#arquitetura)
- [Versionamento](#versionamento)
- [Roadmap](#roadmap)
- [Contribuindo](#contribuindo)
- [Segurança](#segurança)
- [Licença](#licença)
- [Aviso](#aviso)

---

## Visão Geral

`Asaas.Api` é um SDK .NET **não-oficial** que cobre **100% da API v3 documentada** do Asaas — mais de **150 endpoints** distribuídos em **27 domain managers**. Foi projetado para ser:

- **Type-safe**: enums tipados para todos os status, billing types, frequências e códigos. Sem strings mágicas.
- **Schema-first**: cada modelo é verificado endpoint-a-endpoint contra a [especificação OpenAPI oficial via MCP](https://docs.asaas.com/mcp). Contract tests congelam o shape JSON.
- **Sem dependências externas**: usa apenas `System.Text.Json` (built-in do .NET 10).
- **Production-ready**: 664 testes unit/contract + 15 integration tests reais contra `api-sandbox.asaas.com` rodando em CI nightly.

Veja [CONFORMANCE.md](CONFORMANCE.md) para o relatório de auditoria endpoint-a-endpoint.

## Destaques

- **27 managers prontos para usar** — Customer, Payment, Subscription, Installment, Pix (estático, automático, recorrente), Webhook, Invoice, Anticipation, Transfer, PaymentLink, CreditCard, PaymentDunning, BillPayment, MobilePhoneRecharge, Notification, FiscalInfo, Finance, MyAccount, AsaasAccount, Wallet, CreditBureauReport, Chargeback, Escrow, Checkout, Sandbox.
- **Modelos auditados contra o schema oficial** — 42 famílias de bugs corrigidos na auditoria v3.2.0 (campos inventados removidos, enums incompletos completados, nullables corrigidos, casing de filtros padronizado).
- **Cobertura de testes em três camadas:**
  - Unit tests para cada manager (chamada HTTP correta, route, método)
  - Contract tests com fixtures dos exemplos oficiais MCP (shape JSON congelado)
  - Integration tests opcionais contra sandbox real (skip automático sem token)
- **Robustez de runtime:** `bool?` em filtros serializa lowercase (`true`/`false`, não `True`/`False`), `decimal?` usa `InvariantCulture` (`12.5`, não `12,5` em pt-BR), `DateTime?` usa formato ISO em qualquer cultura.
- **Sem mocks frágeis:** o test runner real (Moq + MockHttpMessageHandler) injeta resposta HTTP no `BaseManager`, validando o JSON exato que sairia na rede.
- **CI pronto:** workflows GitHub Actions para build/test em todo PR e workflow separado de integration tests com secret `ASAAS_SANDBOX_TOKEN`.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) ou superior
- Conta Asaas com `access_token` (sandbox ou produção)

## Instalação

```bash
dotnet add package Asaas.Api
```

```powershell
Install-Package Asaas.Api
```

```xml
<PackageReference Include="Asaas.Api" Version="3.2.0" />
```

## Quick Start

```csharp
using Codout.Apis.Asaas;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Customer;
using Codout.Apis.Asaas.Models.Payment;

// 1. Configure
var settings = new ApiSettings(
    accessToken: Environment.GetEnvironmentVariable("ASAAS_TOKEN")!,
    applicationName: "MeuApp/1.0",
    asaasEnvironment: AsaasEnvironment.SANDBOX);

var asaas = new AsaasApi(settings);

// 2. Crie um cliente
var customer = await asaas.Customer.Create(new CreateCustomerRequest
{
    Name      = "Maria da Silva",
    CpfCnpj   = "01020558075",
    Email     = "maria@example.com"
});

if (!customer.WasSuccessful())
{
    foreach (var error in customer.Errors)
        Console.WriteLine($"[{error.Code}] {error.Description}");
    return;
}

// 3. Crie uma cobrança Pix
var payment = await asaas.Payment.Create(new CreatePaymentRequest
{
    CustomerId   = customer.Data.Id,
    BillingType  = BillingType.PIX,
    Value        = 150.00m,
    DueDate      = DateTime.UtcNow.Date.AddDays(7),
    Description  = "Pedido #1234"
});

// 4. Obtenha o QR Code Pix
var qr = await asaas.Payment.GetPixQrCode(payment.Data.Id);
Console.WriteLine(qr.Data.Payload);          // copia-e-cola
Console.WriteLine(qr.Data.EncodedImage);     // base64 PNG
```

## Conceitos Centrais

### Ambientes

| Ambiente | Enum | Base URL |
|---|---|---|
| Sandbox | `AsaasEnvironment.SANDBOX` | `https://api-sandbox.asaas.com/v3` |
| Produção | `AsaasEnvironment.PRODUCTION` | `https://api.asaas.com/v3` |

Use sempre **Sandbox** durante desenvolvimento. O manager `asaas.Sandbox` lança `InvalidOperationException` se for chamado em ambiente de produção.

### Responses tipadas

Toda chamada retorna `ResponseObject<T>` (item único) ou `ResponseList<T>` (paginada). Ambos expõem `WasSuccessful()`, `StatusCode` e `Errors`:

```csharp
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
        Console.WriteLine($"[{error.Code}] {error.Description}");
}
```

```csharp
ResponseList<Payment> list = await asaas.Payment.List(offset: 0, limit: 10);

Console.WriteLine($"Total: {list.TotalCount} — HasMore: {list.HasMore}");
foreach (var payment in list.Data)
    Console.WriteLine($"{payment.Id} — R$ {payment.Value} — {payment.Status}");
```

### Filtros e paginação

A maioria dos `List` aceita um filtro opcional. Todos os filtros são objetos tipados que herdam de `RequestParameters`:

```csharp
var payments = await asaas.Payment.List(0, 20, new PaymentListFilter
{
    CustomerId   = "cus_123",
    BillingType  = BillingType.PIX,
    Status       = PaymentStatus.PENDING,
    DueDateGE    = DateTime.UtcNow.Date,
    DueDateLE    = DateTime.UtcNow.Date.AddMonths(1),
    Anticipated  = false
});
```

`bool?`, `DateTime?`, `decimal?` e enums são serializados no formato exato que a API exige, com fallback para `InvariantCulture` (resolve o bug clássico de `12,5` em vez de `12.5` em pt-BR).

### Tratamento de erros

```csharp
var result = await asaas.Payment.Create(request);

if (result.StatusCode == HttpStatusCode.BadRequest)
{
    // Validação de regras de negócio do Asaas
    var invalidCustomer = result.Errors.FirstOrDefault(e => e.Code == "invalid_customer");
    if (invalidCustomer is not null)
        // cliente não existe ou não tem cobrança permitida
        ...
}

if (result.StatusCode == HttpStatusCode.Unauthorized)
    // access_token inválido ou expirado
    ...

if (result.StatusCode == HttpStatusCode.TooManyRequests)
    // backoff exponencial recomendado
    ...
```

### Cultura e serialização

A serialização JSON usa configuração centralizada em `Core/JsonSerializerConfiguration.cs`:

- **camelCase** automático (`CustomerId` → `customerId`)
- **`null`-ignoring** (campos não setados não vão na requisição)
- **enums como string** (com `SafeEnumConverterFactory` que aceita valores novos sem quebrar)
- **datas flexíveis** (aceita `2025-01-01`, `2025-01-01T10:00:00`, `2025-01-01 10:00:00`, etc.)

Query strings (filtros) usam `RequestParameters` que sempre serializa em `InvariantCulture`, independente da cultura do thread.

## Managers Disponíveis

| Manager | Propriedade | Endpoints | Descrição |
|---|---|---|---|
| Clientes | `asaas.Customer` | 7 | CRUD + notificações |
| Cobranças | `asaas.Payment` | 28+ | Boleto/Pix/Cartão + splits + refunds + documents + billing/viewing info + simulate + limits |
| Parcelamentos | `asaas.Installment` | 10 | CRUD + refund + splits + paymentBook |
| Assinaturas | `asaas.Subscription` | 12 | CRUD + creditCard + invoiceSettings + payments |
| Links de pagamento | `asaas.PaymentLink` | 11 | CRUD + imagens |
| Pix | `asaas.Pix` | 13 | AddressKeys, QrCodes, Transactions (com filter), tokenBucket, decode, pay |
| Pix Automático | `asaas.PixAutomatic` | 6 | Autorizações recorrentes Bacen + payment instructions |
| Pix Recorrente | `asaas.PixRecurring` | 5 | Recorrências parceladas + itens |
| Webhooks | `asaas.Webhook` | 6 | CRUD + removeBackoff (~100 eventos disponíveis) |
| Notas Fiscais | `asaas.Invoice` | 7 | Schedule, Find, List (com 7 filtros), Update, Authorize, Cancel |
| Informações Fiscais | `asaas.FiscalInfo` | 4 | CreateOrUpdate + Find + lookups municipais |
| Financeiro | `asaas.Finance` | 4 | Balance + Statistics (com filter) + Split |
| Transferências | `asaas.Transfer` | 5 | Para Asaas/banco/Pix + Find + List + Cancel |
| Antecipações | `asaas.Anticipation` | 8 | CRUD + Simulate + Limits + Automatic configuration |
| Negativações | `asaas.PaymentDunning` | 9 | CRUD + Simulate + History + PartialPayments + ResendDocument |
| Pagamento de contas | `asaas.BillPayment` | 5 | CRUD + Simulate + Cancel |
| Recarga celular | `asaas.MobilePhoneRecharge` | 5 | CRUD + GetProvider |
| Notificações | `asaas.Notification` | 2 | Update + BatchUpdate |
| Cartão de crédito | `asaas.CreditCard` | 3 | TokenizeCreditCard + PreAuthorization config |
| Contas Asaas (subaccounts) | `asaas.AsaasAccount` | 6 | CRUD + ResendActivationLink + AccessTokens |
| Minha conta | `asaas.MyAccount` | 10 | CommercialInfo + Status + Fees + AccountNumber + PaymentCheckoutConfig + Documents |
| Carteiras | `asaas.Wallet` | 1 | List |
| Consulta SERASA | `asaas.CreditBureauReport` | 3 | Create + Find + List (com filter) |
| Chargebacks | `asaas.Chargeback` | 3 | List + FindByPayment + CreateDispute |
| Escrow | `asaas.Escrow` | 6 | Configuração subconta/padrão + payment escrow |
| Checkout | `asaas.Checkout` | 2 | Create + Cancel |
| Sandbox | `asaas.Sandbox` | 3 | ApproveAccount + ConfirmPayment + ForceOverdue (bloqueado em PRODUCTION) |

Lista completa de endpoints por manager: [CONFORMANCE.md §1–§29](CONFORMANCE.md).

## Exemplos por Domínio

### Cobranças (Pix / Boleto / Cartão)

```csharp
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment;

// Pix
var pix = await asaas.Payment.Create(new CreatePaymentRequest
{
    CustomerId  = "cus_123",
    BillingType = BillingType.PIX,
    Value       = 150m,
    DueDate     = DateTime.UtcNow.Date.AddDays(7),
    Description = "Pedido #1234"
});

// Boleto com multa e juros
var boleto = await asaas.Payment.Create(new CreatePaymentRequest
{
    CustomerId  = "cus_123",
    BillingType = BillingType.BOLETO,
    Value       = 1000m,
    DueDate     = DateTime.UtcNow.Date.AddDays(7),
    Discount    = new Discount { Value = 50m, DueDateLimitDays = 3, Type = DiscountType.FIXED },
    Interest    = new Interest { Value = 2m },   // % ao mês
    Fine        = new Fine     { Value = 1m, Type = FineType.PERCENTAGE }
});

// Cartão tokenizado (em endpoint dedicado)
var withCard = await asaas.Payment.CreateWithCreditCard(new CreatePaymentRequest
{
    CustomerId      = "cus_123",
    BillingType     = BillingType.CREDIT_CARD,
    Value           = 299.90m,
    DueDate         = DateTime.UtcNow.Date,
    CreditCardToken = "tok_a75a1d98-c52d-4a6b",   // tokenizado previamente
    RemoteIp        = "200.123.45.67"
});
```

### Assinaturas

```csharp
using Codout.Apis.Asaas.Models.Subscription;
using Codout.Apis.Asaas.Models.Subscription.Enums;

var sub = await asaas.Subscription.Create(new CreateSubscriptionRequest
{
    CustomerId   = "cus_123",
    BillingType  = BillingType.CREDIT_CARD,
    Value        = 99.90m,
    NextDueDate  = DateTime.UtcNow.Date.AddDays(30),
    Cycle        = Cycle.MONTHLY,
    Description  = "Plano Premium",
    CreditCard   = new CreditCardRequest { /* ... */ },
    CreditCardHolderInfo = new CreditCardHolderInfoRequest { /* ... */ }
});

// Listar cobranças geradas pela assinatura
var payments = await asaas.Subscription.ListPayments("sub_123", 0, 20);

// Filtros novos (v3.2.0 — B-27e)
var filtered = await asaas.Subscription.List(0, 50, new SubscriptionListFilter
{
    Status         = SubscriptionStatus.INACTIVE,
    DeletedOnly    = false,
    Sort           = "dateCreated",
    Order          = "desc"
});
```

### Links de pagamento

```csharp
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;

var link = await asaas.PaymentLink.Create(new CreatePaymentLinkRequest
{
    Name              = "Produto XYZ",
    Value             = 199.90m,
    BillingType       = BillingType.UNDEFINED,
    ChargeType        = ChargeType.DETACHED,
    DueDateLimitDays  = 10
});

Console.WriteLine($"Compartilhe: {link.Data.Url}");
```

### Pix

```csharp
using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;

// Criar QR Code estático
var qr = await asaas.Pix.CreateStaticQrCode(new CreatePixStaticQrCodeRequest
{
    AddressKey  = "minha-chave-pix",
    Description = "Doação",
    Value       = 50m
});

// Criar chave EVP (aleatória)
var key = await asaas.Pix.CreateAddressKey(new CreatePixAddressKeyRequest
{
    Type = PixAddressKeyType.EVP
});

// Listar transações com filtro de status (v3.2.0 — novo)
var txs = await asaas.Pix.ListTransactions(0, 20, new PixTransactionListFilter
{
    Status = PixTransactionStatus.AWAITING_REQUEST
});
```

### Webhooks

```csharp
using Codout.Apis.Asaas.Models.Webhook;
using Codout.Apis.Asaas.Models.Webhook.Enums;

var webhook = await asaas.Webhook.Create(new CreateWebhookRequest
{
    Name        = "Notificações de pagamento",
    Url         = "https://meusite.com/webhook/asaas",
    Email       = "ops@meusite.com",
    Enabled     = true,
    Interrupted = false,
    ApiVersion  = 3,
    AuthToken   = "whsec_min_32_caracteres_para_assinar_callbacks",
    SendType    = WebhookSendType.SEQUENTIALLY,
    Events =
    [
        WebhookEvent.PAYMENT_CONFIRMED,
        WebhookEvent.PAYMENT_RECEIVED,
        WebhookEvent.PAYMENT_OVERDUE,
        WebhookEvent.PAYMENT_REFUNDED
    ]
});
```

> **Nota:** Este SDK expõe `WebhookManager` para configurar endpoints, mas não fornece decoders tipados para os payloads que o Asaas envia para a sua URL. A deserialização do payload recebido fica a cargo do consumidor.

### Transferências

```csharp
using Codout.Apis.Asaas.Models.Transfer;
using Codout.Apis.Asaas.Models.Transfer.Enums;

// Para outra conta Asaas (POST /v3/transfers/)
await asaas.Transfer.TransferToAsaasAccount(new AsaasAccountTransferRequest
{
    WalletId = "wallet_destino",
    Value    = 500m
});

// Para banco externo (TED ou Pix — POST /v3/transfers)
await asaas.Transfer.TransferToBankAccount(new BankAccountTransferRequest
{
    Value = 1000m,
    BankAccount = new BankAccount
    {
        Bank             = new Bank { Code = "341" },
        OwnerName        = "Empresa LTDA",
        CpfCnpj          = "12345678000100",
        Agency           = "1234",
        Account          = "56789",
        AccountDigit     = "0",
        BankAccountType  = BankAccountType.CONTA_CORRENTE
    }
});

// Listar com filtros de data (v3.2.0 — B-29h)
var list = await asaas.Transfer.List(0, 50, new TransferListFilter
{
    DateCreatedGE = DateTime.UtcNow.Date.AddDays(-30),
    DateCreatedLE = DateTime.UtcNow.Date
});
```

### Notas fiscais (NFS-e)

```csharp
using Codout.Apis.Asaas.Models.Invoice;
using Codout.Apis.Asaas.Models.Common;

var invoice = await asaas.Invoice.Schedule(new CreateInvoiceRequest
{
    PaymentId            = "pay_123",
    ServiceDescription   = "Serviço de consultoria",
    Observations         = "Referente ao mês de janeiro",
    Value                = 1500m,
    Deductions           = 0m,
    EffectiveDate        = DateTime.UtcNow.Date,
    MunicipalServiceName = "Consultoria em TI",
    Taxes = new Taxes
    {
        RetainIss = false,
        Iss = 2m, Pis = 0.65m, Cofins = 3m,
        Csll = 1m, Inss = 0m, Ir = 1.5m
    }
});

await asaas.Invoice.Authorize(invoice.Data.Id);

// Lookup de códigos municipais
var services = await asaas.FiscalInfo.ListServices(description: "consultoria");
```

### Saldo e estatísticas

```csharp
var balance = await asaas.Finance.GetBalance();
Console.WriteLine($"Saldo: R$ {balance.Data.Value}");

// Estatísticas com filtros (v3.2.0 — B-37b)
var stats = await asaas.Finance.GetPaymentStatistics(new PaymentStatisticsFilter
{
    BillingType    = BillingType.PIX,
    Status         = PaymentStatus.RECEIVED,
    DateCreatedGE  = DateTime.UtcNow.Date.AddDays(-30),
    DateCreatedLE  = DateTime.UtcNow.Date
});

Console.WriteLine($"Quantidade: {stats.Data.Quantity} — Total: R$ {stats.Data.Value}");

// Split a receber/enviar (v3.2.0 — schema corrigido B-37a)
var split = await asaas.Finance.GetSplitStatistics();
Console.WriteLine($"A receber: R$ {split.Data.Income} — A enviar: R$ {split.Data.Value}");
```

### Sandbox (apenas testes)

```csharp
// Em AsaasEnvironment.SANDBOX você pode acelerar fluxos de teste:
await asaas.Sandbox.ApproveAccount();           // aprova conta sandbox
await asaas.Sandbox.ConfirmPayment("pay_123");  // confirma pagamento sem boleto/pix
await asaas.Sandbox.ForceOverdue("pay_123");    // força vencimento imediato

// Em PRODUCTION todos lançam InvalidOperationException antes de fazer HTTP.
```

## Configuração Avançada

### Timeout customizado

```csharp
var settings = new ApiSettings("TOKEN", "MeuApp/1.0", AsaasEnvironment.PRODUCTION)
{
    TimeOut = TimeSpan.FromSeconds(60)
};
```

### User-Agent

O parâmetro `applicationName` em `ApiSettings` se torna parte do `User-Agent` da requisição, ajudando o suporte Asaas a identificar a integração em caso de troubleshooting:

```csharp
new ApiSettings("TOKEN", "ECommerceXYZ/2.5.0", AsaasEnvironment.PRODUCTION);
// User-Agent enviado: ECommerceXYZ/2.5.0
```

### Reaproveitamento de conexões

`BaseManager` usa um `SocketsHttpHandler` estático e compartilhado por toda a instância de `AsaasApi`. Recomenda-se manter um único `AsaasApi` como singleton ao longo do ciclo de vida da aplicação (registrar no DI com `AddSingleton`).

```csharp
// ASP.NET Core
builder.Services.AddSingleton(_ => new AsaasApi(new ApiSettings(
    accessToken:       builder.Configuration["Asaas:Token"]!,
    applicationName:   "MeuApp/1.0",
    asaasEnvironment:  AsaasEnvironment.PRODUCTION)));
```

## Qualidade e Conformidade

Esta biblioteca passa por **auditoria schema-first contínua** contra o [MCP oficial do Asaas](https://docs.asaas.com/mcp), garantindo que cada modelo, enum, filtro e nome de campo bate exatamente com a especificação OpenAPI publicada.

| Métrica | Valor |
|---|---|
| Managers auditados | **27 / 27** (100%) |
| Endpoints cobertos | ~150 |
| Famílias de bugs corrigidos | **42** (B-19 a B-42) |
| Testes unit + contract | **664** passando |
| Integration tests sandbox | **15 / 15** passando |
| Warnings de build | **0** |

Relatório completo: [CONFORMANCE.md](CONFORMANCE.md) — inclui tabela endpoint-a-endpoint por manager, lista de bugs por padrão (envelope, casing, nullable, enums inventados, etc.) e riscos remanescentes classificados.

## Testes

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Build

```bash
dotnet build Codout.Apis.Asaas/
```

### Suite local (unit + contract)

```bash
# Roda todos os testes que não dependem do sandbox
dotnet test Codout.Apis.Asaas.Tests/ --filter "Category!=Integration"
```

```bash
# Tests de um manager específico
dotnet test Codout.Apis.Asaas.Tests/ --filter "FullyQualifiedName~CustomerManagerTests"
```

### Integration tests (sandbox real)

Integration tests fazem chamadas reais contra `api-sandbox.asaas.com`. São **puladas automaticamente** quando a variável de ambiente `ASAAS_SANDBOX_TOKEN` está vazia — o CI local não quebra:

```powershell
$env:ASAAS_SANDBOX_TOKEN = "aact_YTU0..."
dotnet test Codout.Apis.Asaas.Tests/ --filter "Category=Integration"
```

### CI

O repositório expõe dois workflows GitHub Actions:

- [`ci.yml`](.github/workflows/ci.yml) — roda em todo push/PR. Build + testes unit/contract (integration filtrados fora).
- [`integration-sandbox.yml`](.github/workflows/integration-sandbox.yml) — dispatch manual + nightly às 04:00 UTC. Usa o secret `ASAAS_SANDBOX_TOKEN` configurado no repositório.

### Empacotar localmente

```bash
dotnet pack Codout.Apis.Asaas/ -c Release
```

## Arquitetura

```
Codout.Apis.Asaas/
├── AsaasApi.cs                       # Facade — entry point com 27 managers via Lazy<T>
├── Core/
│   ├── ApiSettings.cs                # Configuração (token, ambiente, timeout, app name)
│   ├── BaseManager.cs                # Base HTTP (GET/POST/PUT/DELETE), SocketsHttpHandler estático
│   ├── JsonSerializerConfiguration.cs # camelCase + SafeEnumConverterFactory + FlexibleDateTimeConverter
│   ├── RequestParameters.cs          # Query string builder (InvariantCulture, bool lowercase)
│   ├── Response/
│   │   ├── Base/BaseResponse.cs      # StatusCode + Errors + WasSuccessful()
│   │   ├── ResponseObject<T>.cs
│   │   └── ResponseList<T>.cs        # envelope padrão {object, hasMore, totalCount, limit, offset, data}
│   └── Extension/                    # DateTime, StatusCode, String helpers
├── Managers/                         # 27 domain managers
└── Models/                           # Request/Response por domínio (1 classe por arquivo)
    ├── Customer/
    ├── Payment/
    ├── Pix/
    ├── ...
    └── Common/                       # Discount, Interest, Fine, CreditCard, Split, etc.
```

## Versionamento

Este projeto segue [Versionamento Semântico](https://semver.org/lang/pt-BR/):

- **MAJOR** (`x.0.0`): mudanças incompatíveis — campos removidos, tipos alterados, métodos renomeados.
- **MINOR** (`3.x.0`): novos endpoints, novos campos opcionais, novos filtros, bug fixes que mudem tipo de campo opcional (e.g. `string` → `enum?`).
- **PATCH** (`3.2.x`): bug fixes não-comportamentais, melhorias de doc, cleanup.

Cada release tem uma entrada detalhada no [CHANGELOG.md](CHANGELOG.md) com:

- Lista de breaking changes por modelo
- Bugs corrigidos com referência ao código B-XX rastreável
- Métricas de testes

## Roadmap

Itens não-bloqueantes que podem entrar em releases futuras:

- Decoders tipados para os payloads de webhook recebidos do Asaas (atualmente, consumidores deserializam manualmente).
- Lookups adicionais do FiscalInfo (`federalServiceCodes`, `nbsCodes`, `operationIndicatorCodes`, `taxClassificationCodes`, `taxSituationCodes`, `nationalPortal`).
- Auditoria contínua quando a spec da Reforma Tributária estabilizar.

## Contribuindo

Contribuições são bem-vindas! Para mudanças significativas, abra uma issue primeiro para discutir o que você gostaria de mudar.

1. Faça fork do repositório.
2. Crie uma branch para sua feature ou fix (`git checkout -b feature/minha-feature`).
3. **Audite o(s) endpoint(s) afetado(s) contra o MCP oficial.** Adicione/atualize o fixture em `Codout.Apis.Asaas.Tests/Fixtures/` e o contract test correspondente.
4. Garanta que `dotnet test --filter "Category!=Integration"` passa.
5. Atualize [CONFORMANCE.md](CONFORMANCE.md) com o(s) bug(s) corrigido(s) seguindo a numeração B-XX.
6. Atualize [CHANGELOG.md](CHANGELOG.md) com a entrada.
7. Abra um Pull Request descrevendo a mudança e referenciando o(s) endpoint(s) MCP consultado(s).

## Segurança

Se você descobrir uma vulnerabilidade de segurança, **não abra uma issue pública**. Em vez disso, envie um email para o autor (veja [LICENSE](LICENSE) para contato) ou abra um [security advisory privado](https://github.com/Codout/Codout.Apis.Asaas/security/advisories/new).

Por convenção:

- Tokens da Asaas (`access_token`, `ASAAS_SANDBOX_TOKEN`) nunca devem ser commitados.
- O `User-Agent` enviado pela biblioteca não inclui PII.
- Logs gerados pelo SDK não incluem o `access_token`.

## Licença

[MIT](LICENSE) © [Clovis Coli Jr](https://github.com/cloviscoli) / [Codout](https://codout.com)

## Aviso

Esta é uma biblioteca **não-oficial**. Não possui vínculo, endosso ou suporte oficial da Asaas. Para suporte oficial da API, consulte a [documentação do Asaas](https://docs.asaas.com) ou o [Help Center](https://ajuda.asaas.com).

Para questões específicas deste SDK, use as [issues do GitHub](https://github.com/Codout/Codout.Apis.Asaas/issues).
