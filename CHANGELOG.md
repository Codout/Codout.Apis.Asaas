# Changelog

Todas as mudancas notaveis deste projeto serao documentadas neste arquivo.

O formato e baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
e este projeto adere ao [Versionamento Semantico](https://semver.org/lang/pt-BR/).

## [3.3.0] - 2026-06-26 — Onboarding de subcontas (white-label/BaaS)

Ajustes para o fluxo de criação de subcontas via API e suporte a testes contra
mocks locais. Todas as mudanças são **aditivas** (não-breaking): novos campos
opcionais e um parâmetro de construtor opcional com default.

### Adicionado

**AsaasAccount (criação de subconta):**
- `CreateAccountRequest.IncomeValue` (`decimal?`) — faturamento/renda mensal em
  BRL. **Obrigatório no Asaas desde 2024** para criar subcontas.
- `CreateAccountRequest.BirthDate` (`string`, formato `yyyy-MM-dd`) — data de
  nascimento do titular. Obrigatório para pessoa física; omitir para PJ.

**Infraestrutura:**
- `ApiSettings.BaseUrl` (`string`, opcional) — override da URL base. Quando
  informado, sobrepõe o ambiente (`AsaasEnvironment`) e é usado em
  `BaseManager.BuildBaseAddress()`. Uso típico: apontar para um mock local
  (ex.: Mockoon) em testes. Novo parâmetro opcional `baseUrl = null` no
  construtor de `ApiSettings` (compatível com o uso existente).

## [3.2.0] - 2026-05-24 — Auditoria schema-first **completa** (27/27 managers)

Segunda rodada da auditoria, agora cobrindo os 16 managers restantes que tinham
ficado de fora da v3.1.0: Customer, Payment (resto), Subscription, Pix, Transfer,
Anticipation, Installment, Webhook, Wallet, Notification, CreditCard, PaymentLink,
Finance, MyAccount (resto), AsaasAccount, FiscalInfo, Chargeback, Sandbox.

Veja [CONFORMANCE.md](CONFORMANCE.md) §12–§29 para o relatório endpoint-a-endpoint
e §50 para o consolidado de padrões de bug por categoria.

### Breaking changes (modelos)

**Pix (CRÍTICO):**
- `PixTransactionStatus` enum tinha 5 valores INVENTADOS (PENDING, DONE, CANCELLED,
  SCHEDULED, FAILED). Schema real: 11 valores. PENDING e FAILED **não existem**.
  Sem o fix, deserializar JSON real do sandbox lançava exception em qualquer
  transacao em estado de espera (AWAITING_*, REQUESTED, REFUSED).
- `PixTransaction` reescrito (7 → 25 campos). Renomeados: `TransactionDate` →
  `EffectiveDate`, `ScheduleDate` → `ScheduledDate`.
- `PixAddressKey.Status` string → enum `PixAddressKeyStatus` (6 valores).
  Adicionados QrCode (nested), CanBeDeleted, CannotBeDeletedReason.
- Novos enums: `PixTransactionType` (5), `PixTransactionOriginType` (6),
  `PixTransactionFinality` (2), `PixAddressKeyStatus` (6).
- Novo filtro: `PixTransactionListFilter` (status, type, endToEndIdentifier).

**Customer:**
- `Customer.DateCreated` → DateTime?
- `Create/UpdateCustomerRequest.NotificationDisabled` → bool?
  (antes forçava false em todo update parcial).
- Novo: `CustomerManager.GetNotifications(customerId)` (endpoint estava faltando).

**Payment:**
- `Payment.DateCreated`, `DueDate`, `OriginalDueDate` → DateTime?
- `PaymentListFilter`: 9 filtros novos (customerGroupName, invoiceStatus,
  estimatedCreditDate, pixQrCodeId, anticipable, user, checkoutSession,
  dateCreated[ge]/[le], estimatedCreditDate[ge]/[le]).

**Subscription:**
- `SubscriptionStatus` enum: adicionado `INACTIVE` (3 valores).
- `Subscription.DateCreated`, `NextDueDate` → DateTime?
- Adicionados: Object, PaymentLinkId, CheckoutSession, Split (array).
- `SubscriptionListFilter`: 6 campos novos (customerGroupName, status enum,
  deletedOnly, externalReference, order, sort).

**Transfer:**
- `AsaasAccountTransferStatus` enum: 3 → 5 valores (adicionados BANK_PROCESSING,
  FAILED).
- `BaseTransfer.DateCreated` → DateTime?, `Authorized` → bool?
- Novo enum: `TransferOperationType` (PIX/TED/INTERNAL).
- `BaseTransfer` adicionados: Object, NetValue (movido), EndToEndIdentifier,
  FailReason, ExternalReference, Description, Recurring.
- `TransferListFilter`: dateCreated[ge]/[le], transferDate[ge]/[le].
- `Bank` model: adicionados Ispb + Name. `BankAccount`: adicionados AgencyDigit,
  PixAddressKey, Ispb.

**Anticipation:**
- `Anticipation.AnticipationDate`, `DueDate`, `RequestDate` → DateTime?, +Object.

**Installment:**
- `Installment.ExpirationDay` → int?
- Adicionados: CreditCard (nested), Refunds (array de InstallmentRefund).

**Notification:**
- Novo enum `NotificationEvent` (6 valores). Antes não existia.
- Notification + UpdateNotificationRequest: todos os 7 bools → bool?
- Notification adicionados: Object, Event (enum), Deleted.

**CreditCard:**
- `Common.CreditCard.Brand` string → enum `CreditCardBrand` (13 valores).
- `PreAuthorizationConfig` reescrito: tinha {Enabled, AutomaticCaptureDelay}
  INVENTADOS. Schema: {DaysToExpire}. Idem `SavePreAuthorizationConfigRequest`.

**PaymentLink:**
- `PaymentLink.SubscriptionCycle` string → `Cycle` enum (7 valores).
- Adicionados: ViewCount, IsAddressRequired, ExternalReference.
- Value, Active, NotificationEnabled, Deleted, DueDateLimitDays,
  MaxInstallmentCount → nullable.

**Finance:**
- `SplitStatistics` reescrito: tinha {TotalPendingValue, TotalReceivedValue}
  INVENTADOS. Schema: {income, value}.
- Novo filter: `PaymentStatisticsFilter` (11 campos). GetPaymentStatistics
  agora aceita filtros opcionais.

**MyAccount:**
- `MyAccount.Status` string → enum `AccountInfoStatus` (4 valores).
- Adicionados: CompanyName, IncomeValue, TradingName, Site,
  AvailableCompanyNames (array), CommercialInfoExpiration (nested).
- `InscricaoEstadual` marcado [Obsolete] (não existe no schema).

**AsaasAccount:**
- `Account.City` string → long? (schema: integer city id).
- Adicionados: Object, Id, BirthDate, TradingName, Site, AccountNumber (nested),
  CommercialInfoExpiration (nested).
- `ApiKey` marcado [Obsolete] (não existe no schema).

**FiscalInfo:**
- `RpsNumber`, `LoteNumber` string → int? (schema: integer).
- Adicionados: NbsCode, PasswordSent, AccessTokenSent, CertificateSent,
  NationalPortalTaxCalculationRegime, Object.
- `StateInscription`, `AccessToken` marcados [Obsolete] (não existem no schema).
- SimplesNacional, CulturalProjectsPromoter → bool?

**Chargeback:**
- Adicionado `ChargebackCreditCard` (nested: number + brand enum).
- `Reason` → nullable.

**Wallet:**
- Adicionado: Object.

### Added

- 16 novos enums tipados (PixTransactionStatus, PixTransactionType,
  PixTransactionOriginType, PixTransactionFinality, PixAddressKeyStatus,
  SubscriptionStatus expandido, NotificationEvent, CreditCardBrand,
  TransferOperationType, AsaasAccountTransferStatus expandido,
  AccountInfoStatus, BillPaymentStatus expandido em fase anterior, etc).
- Novos models: PixTransactionExternalAccount, PixOriginalTransaction,
  PixTransactionQrCode, PixAddressKeyQrCode, PixTransactionListFilter,
  ChargebackCreditCard, CommercialInfoExpiration, AccountNumber (já existia),
  InstallmentRefund, PaymentStatisticsFilter.
- 10 novos integration tests cobrindo Subscription, Pix, Transfer,
  Anticipation, Finance (15 tests no total).
- CI workflow [`integration-sandbox.yml`](.github/workflows/integration-sandbox.yml):
  workflow_dispatch (manual) + nightly schedule, com secret ASAAS_SANDBOX_TOKEN.
- CI workflow [`ci.yml`](.github/workflows/ci.yml) atualizado para filtrar
  `Category!=Integration` (não quebra sem token).
- CONFORMANCE.md §12–§29 (16 novos managers), §50 (cross-pattern bug list).

### Fixed

- 18 famílias de bugs (B-25 a B-42). Veja CONFORMANCE.md §50 para consolidação
  por padrão.

### Métricas

- Testes: 599 → **664 unit/contract** (+65) + **15 integration** skip-by-default.
- Managers auditados: 11 → **27** (100%).
- Bugs cumulativos: 24 → **42 famílias** (B-19 a B-42).
- Build warnings: 0.

## [3.1.0] - 2026-05-24 — Auditoria schema-first

Rodada final de conformidade contra o MCP oficial Asaas. Para cada manager
auditado, modelos foram verificados campo-a-campo contra OpenAPI, fixtures
criadas a partir dos exemplos oficiais e contract tests congelam o shape JSON.
Veja [CONFORMANCE.md](CONFORMANCE.md) para o relatorio completo endpoint-a-endpoint.

### Breaking changes (modelos)

**PaymentDunning:**
- `DunningNumber: string` -> `int?` (era chute; schema integer).
- `Status: bool` -> `bool?` em `CanBeCancelled` e `IsNecessaryResendDocumentation`
  (schema permite null; valor false silencioso antes do fix).
- `PaymentDunningEventHistory.Status: string` -> enum `PaymentDunningHistoryStatus`.
- `SimulatedPaymentDunning.TypeSimulations` e
  `PaymentDunningPaymentAvailable.TypeSimulations`: objeto unico ->
  `List<PaymentDunningTypeSimulations>` (schema sempre foi array — antes
  lancava `InvalidCastException` no JSON real).
- `Simulate(request)` agora envia `payment` como QUERY param (schema), nao body.
- `ReceivedInCashFeeValue` e `CancellationFeeValue` marcados `[Obsolete]`.
- Adicionados: `CannotBeCancelledReason`, valor enum `DEBT_RECOVERY_ASSISTANCE`
  (em `PaymentDunningType`, aceito pelo filter).

**CreditBureauReport:**
- `CreditBureauReport.State` e `Status` REMOVIDOS (nao existem no schema).
- `CreateCreditBureauReportRequest.State` REMOVIDO.
- Adicionados: `DownloadUrl`, `ReportFile` (PDF Base64 — apenas em response do POST).
- Novo: `CreditBureauReportListFilter` com `StartDate`/`EndDate`. Overload
  `List(offset, limit, filter)` backwards-compatible.

**BillPayment:**
- `BillPayment`: adicionados `Interest`, `Fine`, `PaymentDate`, `ExternalReference`.
  `FailReasons: string` -> `List<string>` (schema array).
  `CanBeCancelled`/`DueDate`/`ScheduleDate`/`PaymentDate` -> nullable.
- `BillPaymentStatus` enum: adicionados `REFUNDED` e
  `AWAITING_CHECKOUT_RISK_ANALYSIS_REQUEST` (5 -> 7 valores).
- `CreateBillPaymentRequest`: adicionados `Interest`, `Fine`, `ExternalReference`.
  `Value`/`DueDate`/`ScheduleDate`/`Discount` -> nullable.
- `BankSlipInfo`: 5 campos com `BankCode` (chute) -> 17 campos com `Bank`,
  `Beneficiary*`, `Min/MaxValue`, `AllowChangeValue`, `Discount/Interest/FineValue`,
  `OriginalValue`, `TotalDiscount/AdditionalValue`, `IsOverdue`.

**Invoice:**
- `Taxes`: 7 campos -> 19 (NBS code, situacao tributaria, classificacao,
  operacao, PIS/COFINS retention type/status + 6 campos da Reforma Tributaria:
  `StateIbs`, `StateIbsValue`, `MunicipalIbs`, `MunicipalIbsValue`, `Cbs`, `CbsValue`).
- `InvoiceListFilter`: `effectiveDate[ge]/[le]` -> `[Ge]/[Le]` (G/L MAIUSCULOS,
  schema oficial). Casing errado era silenciosamente ignorado pela API.
  Adicionados filtros `customer` e `externalReference`.
- `CreateInvoiceRequest` e `UpdateInvoiceRequest`: adicionado `UpdatePayment: bool?`.

**AccountDocument (subgrupo MyAccount):**
- `AccountDocumentFile` REMOVIDO (Name/Url eram inventados; schema retorna
  apenas `{id, status}`).
- `SubmitDocument`, `ViewDocumentFile`, `UpdateDocumentFile` agora retornam
  `AccountDocument` (antes retornavam tipo errado).
- 4 enums tipados criados: `AccountDocumentStatus` (4), `AccountDocumentGroupStatus`
  (5, ganha IGNORED), `AccountDocumentType` (12), `AccountDocumentResponsibleType` (13).
  Status/Type eram `string`/`List<string>` antes.
- `UploadAccountDocumentRequest`: `DocumentType: string` -> `Type: AccountDocumentType?`,
  `File: IAsaasFile` -> `DocumentFile: IAsaasFile` (alinhando com nomes
  multipart "type" e "documentFile" do schema).

**MobilePhoneRecharge:**
- `MobilePhoneProvider.AvailableValues: List<decimal>` (chute) -> `Values:
  List<MobilePhoneProviderValue>` com `{Name, Description, Bonus, MinValue, MaxValue}`
  conforme schema.

**PixAutomatic (B-16/B-17 do REVIEW pre-existente):**
- `PixAutomaticPaymentInstruction.Authorization` virou objeto aninhado com
  `Id`/`EndToEndIdentifier`/`CustomerId`. Adicionados `DueDate`, `EndToEndIdentifier`,
  `PaymentId`, `RefusalReason`. `Status` virou enum
  `PixAutomaticPaymentInstructionStatus` (5 valores).
- `PixAutomaticPaymentInstructionListFilter`: campos `authorization`/`status` ->
  `authorizationId`/`customerId`/`paymentId`/`status` (typed).

### Added

- **CONFORMANCE.md** — relatorio endpoint-a-endpoint da auditoria com tabelas
  por manager, bugs (B-XX) corrigidos, fixtures, contract tests.
- **Integration tests sandbox** (`Codout.Apis.Asaas.Tests/Integration/`):
  5 testes reais contra `api-sandbox.asaas.com`, skip automatico via
  `[IntegrationFact]` quando `ASAAS_SANDBOX_TOKEN` ausente.
- **Contract tests** (`Codout.Apis.Asaas.Tests/Contract/`): 95+ novos testes
  que congelam o shape JSON de request/response com fixtures dos exemplos MCP.
- `PixRecurringTransactionListFilter` (status/value/searchText) — feature
  anteriormente nao exposta.
- Bug pre-existente fixado em paralelo: `Subscription.Enums.Cycle.BIMONTHLY`
  (estava faltando, presente em schemas de Subscription e Checkout).

### Fixed (sistemicos — descobertos antes desta fase)

- `RequestParameters.Add(decimal?)` agora usa `CultureInfo.InvariantCulture`
  (pt-BR estava gerando `12,5` em vez de `12.5`).
- `RequestParameters.Add(bool?)` serializa `true`/`false` lowercase
  (antes `True`/`False` — Asaas ignorava silenciosamente).
- `DateTimeExtensions.ToApiRequest` defensivamente forca `InvariantCulture`.

## [3.0.0] - 2026-05-24

Major release com auditoria completa de conformidade contra a documentacao
oficial do Asaas (via MCP `https://docs.asaas.com/mcp`). Veja `AUDIT.md`,
`IMPLEMENTATION_PLAN.md` e `REVIEW.md` para o relatorio detalhado.

### Breaking changes adicionais (do REVIEW.md)

- `asaas.ReceivableAnticipation` renomeado para `asaas.Anticipation` (alinhar
  com o naming dos demais 26 managers que usam nome curto). Migracao:
  `asaas.ReceivableAnticipation.X` -> `asaas.Anticipation.X`.
- `ReceivableAnticipationStatusExtension` renomeado para
  `AnticipationStatusExtension`.
- `AccountStatus.{CommercialInfo,Documentation,General,BankAccountInfo}`
  passaram de `string` para enum `AccountApprovalStatus`
  (PENDING/APPROVED/REJECTED/AWAITING_APPROVAL).
- `BaseManager._settings` passou de `private` para `protected Settings`
  (convencao PascalCase de protected field). Codigo que herdava de
  `BaseManager` e usava `_settings` precisa migrar para `Settings`.

Cobertura do SDK passou de ~45% (70 endpoints) para 100% (~156 endpoints
documentados). Suite de testes passou de 400 para ~500 testes.

### Breaking changes

**HTTP method:**
- `CustomerManager.Update`, `PaymentManager.Update`, `SubscriptionManager.Update`,
  `SubscriptionManager.UpdateInvoiceSettings`, `NotificationManager.Update`,
  `NotificationManager.BatchUpdate` agora enviam **PUT** (antes era POST, sem efeito).

**Renomeacoes / mudancas de rota:**
- `CustomerFiscalInfoManager` -> `FiscalInfoManager`; classes renomeadas
  (`CustomerFiscalInfo` -> `FiscalInfo`, `CreateCustomerFiscalInfoRequest` ->
  `CreateFiscalInfoRequest`). Rota corrigida: `/v3/customerFiscalInfo` -> `/v3/fiscalInfo`.
  Acesso facade: `asaas.CustomerFiscalInfo` -> `asaas.FiscalInfo`.
- `InvoiceManager.ListMunicipalServices` removido e movido para
  `FiscalInfoManager.ListServices` (rota correta `/v3/fiscalInfo/services`).
- `FinanceManager.Balance()` (retornando `decimal`) renomeado para
  `GetBalance()` retornando `ResponseObject<Balance>` (a API retorna objeto).
- `MyAccountManager.Find()` removido (apontava para rota errada). Substituido por
  `GetCommercialInfo()` (rota `/v3/myAccount/commercialInfo`).
- `TransferManager.Execute(...)` removido (overload ambiguo). Substituido por
  `TransferToBankAccount(...)` (POST `/v3/transfers`) e `TransferToAsaasAccount(...)`
  (POST `/v3/transfers/`, com barra final = endpoint separado).
- `WebhookManager` totalmente reescrito de "webhook unico por tipo" (rotas
  `/v3/webhook`, `/v3/webhook/invoice`, `/v3/webhook/mobilePhoneRecharge` que ja
  nao existem) para CRUD por id em `/v3/webhooks/{id}`. Veja secao de migracao
  no final.
- `MunicipalService.Iss` renomeado para `IssTax` (nome correto na API).

**Remocoes:**
- `AnticipationManager.SignAgreement` e `SignAnticipationAgreementRequest`
  removidos: o endpoint `/v3/anticipations/agreement/sign` nao existe na API.

**Modelos:**
- `Customer.Deleted`, `Customer.NotificationDisabled`, `Payment.Deleted`,
  `Payment.PostalService`, `Payment.Anticipated` convertidos para `bool?`.
- `WebhookRequest` removido (substituido por `CreateWebhookRequest` /
  `UpdateWebhookRequest`).

### Bugs corrigidos

- 7 bugs bloqueantes que faziam chamadas reais falharem (PUT->POST,
  rotas incorretas, shape de resposta errado, endpoint inexistente,
  manager sem metodo de criacao).
- Sockets esgotando em apps de alto trafego: `SocketsHttpHandler` agora
  e compartilhado entre todas as instancias de manager.

### Novos managers (Sprint 4 + 5)

- **ChargebackManager** - 3 endpoints (`/v3/chargebacks/*`)
- **EscrowManager** - 6 endpoints (Conta de Garantia)
- **CheckoutManager** - 2 endpoints (`/v3/checkouts/*`)
- **MobilePhoneRechargeManager** - 5 endpoints (`/v3/mobilePhoneRecharges/*`)
- **SandboxManager** - 3 helpers de teste (`/v3/sandbox/*`, lanca excecao em producao)
- **PixAutomaticManager** - 6 endpoints (`/v3/pix/automatic/*`)
- **PixRecurringManager** - 5 endpoints (`/v3/pix/transactions/recurrings/*`)

### Novos endpoints em managers existentes

- **PaymentManager** (+18 endpoints): documentos (5), simulate, limits,
  billingInfo, viewingInfo, status, refunds, bankSlip/refund,
  captureAuthorizedPayment, payWithCreditCard, createWithCreditCard,
  splits queries paid/received (4).
- **SubscriptionManager** (+1): UpdateCreditCard.
- **InstallmentManager** (+5): Create, CreateWithCreditCard, ListPayments,
  CancelPendingPayments, UpdateSplits.
- **PixManager** (+3): FindTransaction, DeleteStaticQrCode, GetAddressKeyTokenBucket.
- **AnticipationManager** (+4): Cancel, GetLimits, GetAutomaticConfiguration,
  UpdateAutomaticConfiguration.
- **CreditCardManager** (+2): SavePreAuthorizationConfig, GetPreAuthorizationConfig.
- **TransferManager** (+1): Cancel.
- **MyAccountManager** (+8): GetCommercialInfo, UpdateCommercialInfo,
  GetStatus, DeleteWhiteLabelAccount, ListPendingDocuments, SubmitDocument,
  ViewDocumentFile, UpdateDocumentFile, DeleteDocumentFile.
- **AsaasAccountManager** (+6): Find, ResendActivationLink, CreateAccessToken,
  ListAccessTokens, UpdateAccessToken, DeleteAccessToken.
- **FiscalInfoManager** (+1): ListServices.

### Adicionado em modelos

- `Customer`: Object, CityName, StateInscription, Company, GroupName,
  ForeignCustomer.
- `Payment`: Object, PixTransaction, PixQrCodeId, CheckoutSession,
  PaymentLinkId, InstallmentNumber, CreditDate, EstimatedCreditDate,
  TransactionReceiptUrl, NossoNumero, Anticipable, CanBePaidAfterDueDate,
  DaysAfterDueDateToRegistrationCancellation.
- `CreatePaymentRequest`: Callback, PixAutomaticAuthorizationId,
  DaysAfterDueDateToRegistrationCancellation.
- `PaymentStatus`: valor REFUND_IN_PROGRESS.
- `CreateCustomerRequest` / `UpdateCustomerRequest`: Company, ForeignCustomer
  (e GroupName no Update).
- Enums `WebhookEvent` (~100 valores) e `WebhookSendType`.
- Enums `ChargebackStatus`, `ChargebackReason` (32 valores), `ChargebackDisputeStatus`.

### Outras melhorias

- `BaseResponse.WasSucessfull()` corrigido para `WasSuccessful()`. Versao com
  typo mantida como `[Obsolete]` alias temporario.
- `BaseManager` agora usa `SocketsHttpHandler` estatico compartilhado entre
  todas as instancias para nao esgotar sockets em apps de alto trafego.

### Guia de migracao 2.x -> 3.x

```csharp
// PUT agora e usado automaticamente nos Updates - nenhuma mudanca de codigo necessaria
await asaas.Customer.Update("cus_123", req);  // antes: POST, agora: PUT

// FiscalInfo
asaas.CustomerFiscalInfo.X    -> asaas.FiscalInfo.X
new CustomerFiscalInfo()      -> new FiscalInfo()
new CreateCustomerFiscalInfoRequest() -> new CreateFiscalInfoRequest()

// Invoice.ListMunicipalServices movido para FiscalInfo.ListServices
await asaas.Invoice.ListMunicipalServices("IT");
// agora:
await asaas.FiscalInfo.ListServices("IT");

// Finance balance shape mudou
decimal saldo = (await asaas.Finance.Balance()).Data;
// agora:
decimal saldo = (await asaas.Finance.GetBalance()).Data.Value;

// Transfer
await asaas.Transfer.Execute(asaasRequest);  // ambiguo
// agora:
await asaas.Transfer.TransferToAsaasAccount(asaasRequest);
await asaas.Transfer.TransferToBankAccount(bankRequest);

// MyAccount.Find -> GetCommercialInfo
var info = (await asaas.MyAccount.Find()).Data;
// agora:
var info = (await asaas.MyAccount.GetCommercialInfo()).Data;

// Webhook completamente novo
// Antes (nao funcionava mais em nenhuma versao da API):
await asaas.Webhook.CreateOrUpdatePaymentWebhook(new WebhookRequest { ... });
// Agora:
await asaas.Webhook.Create(new CreateWebhookRequest
{
    Name = "Meu webhook",
    Url = "https://example.com/hook",
    Email = "ops@example.com",
    Enabled = true,
    ApiVersion = 3,
    AuthToken = "whsec_min32chars......",
    SendType = WebhookSendType.SEQUENTIALLY,
    Events = [WebhookEvent.PAYMENT_CONFIRMED, WebhookEvent.PAYMENT_RECEIVED]
});

// AnticipationManager.SignAgreement removido - se voce usava esse metodo,
// agora o termo de antecipacao e assinado direto no painel web do Asaas.
```

## [2.0.2] - 2026-04-27

### Adicionado
- **CreditCardToken** em `CreateSubscriptionRequest`: agora e possivel criar uma assinatura recorrente reutilizando um token de cartao previamente gerado por `TokenizeCreditCard`, sem precisar enviar novamente os dados sensiveis (numero/CCV). Paridade com `CreatePaymentRequest`, que ja expunha esta propriedade.

## [2.0.1] - 2026-02-19

### Corrigido
- **DateTime deserialization**: Criado `FlexibleDateTimeConverter` que aceita tanto formato date-only (`yyyy-MM-dd`) quanto ISO 8601 completo (`yyyy-MM-ddTHH:mm:ssZ`), resolvendo `JsonException` em campos como `expirationDate` retornados pela API Asaas.
- **Abstract type deserialization**: Removido `abstract` de `BaseDeleted` e `BaseTransfer`, permitindo que `System.Text.Json` desserialize respostas de operacoes DELETE e listagem de transferencias.
- **Enum deserialization**: Substituido `JsonStringEnumConverter` pelo `SafeEnumConverterFactory` que retorna o valor default em vez de lancar `JsonException` quando a API retorna um valor de enum nao mapeado no SDK.
- **NullReferenceException em listas**: Todas as propriedades `List<T>` nos models agora sao inicializadas com listas vazias, evitando `NullReferenceException` ao iterar respostas da API.

### Adicionado
- `ExternalReference` nos models `Invoice`, `CreateInvoiceRequest` e `UpdateInvoiceRequest`.
- `WhatsappEnabledForCustomer` no model `UpdateNotificationRequest`.

## [2.0.0] - 2026-02-19

### Adicionado
- Novos modulos: `PaymentLinkManager`, `PixManager`, `NotificationManager`, `CreditBureauReportManager`, `CustomerFiscalInfoManager`.
- Novos endpoints em managers existentes: `UndoReceivedInCash`, `ListPaymentBook`, `ListPayments`, `Find` (transfer), `SignAgreement`, `PaymentStatistics`, `SplitStatistics`, `FindAccountNumber`, webhooks para `MobilePhoneRecharge`.
- Suporte a `PUT` via `PutAsync<T>` no `BaseManager`.
- 400 testes unitarios com xUnit + Moq.
- README completo com exemplos de uso.
- Configuracao de pacote NuGet com SourceLink e symbol packages (.snupkg).

### Alterado
- **Breaking**: Migrado de `Newtonsoft.Json` para `System.Text.Json` (zero dependencias externas).
- Corrigido path de `Balance` para `/finance/balance`.
- `Invoice Update` agora usa `PUT` corretamente em vez de `POST`.
- `BuildHttpClient()` agora e `protected virtual` para permitir mocking em testes.

## [1.0.0] - 2024-01-01

### Adicionado
- Release inicial do SDK.
- Managers: Customer, Payment, Subscription, Installment, Finance, Transfer, Wallet, Webhook, AsaasAccount, Anticipation, MyAccount, Invoice, PaymentDunning, BillPayment, CreditCard.
- Suporte a ambientes Production e Sandbox.
- Serializacao com Newtonsoft.Json.
