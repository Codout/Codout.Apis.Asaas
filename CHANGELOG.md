# Changelog

Todas as mudancas notaveis deste projeto serao documentadas neste arquivo.

O formato e baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
e este projeto adere ao [Versionamento Semantico](https://semver.org/lang/pt-BR/).

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
