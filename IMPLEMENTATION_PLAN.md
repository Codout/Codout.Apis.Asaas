# Plano de Implementação — Auditoria de Conformidade Asaas SDK

> **Branch:** `audit/asaas-api-conformance`
> **Branch base:** `master`
> **Versão alvo:** `3.0.0` (breaking changes diretos, sem deprecation period)
> **Estratégia:** branch monolítica com commits atômicos (1 commit ≈ 1 PR conceitual)
> **Cobertura de testes:** ampla — cada fix e cada endpoint novo ganha teste

---

## Princípios desta entrega

1. **Cada commit deve compilar e passar nos testes existentes** — facilita bisect e revisão.
2. **Cada bug fix tem ao menos um teste de regressão** que falharia antes do fix e passa depois (usar `AssertRequestMethod` e `AssertRequestUrl`).
3. **Cada endpoint novo tem ao menos um teste de happy path + 1 de erro**.
4. **Tipos de request/response novos** ganham teste de serialização em [SerializationTests.cs](Codout.Apis.Asaas.Tests/Models/SerializationTests.cs).
5. **Breaking changes vão direto** — sem `[Obsolete]`, sem alias. Versão 3.0.0 é o sinal.
6. **Mensagens de commit em português**, padrão Conventional Commits (`fix:`, `feat:`, `refactor:`, `feat!:` para breaking, `BREAKING CHANGE:` no rodapé quando aplicável).
7. **Manter zero dependências externas** — toda nova feature usa só `System.Text.Json` + BCL.

---

## Sprint 1 — Bugs Bloqueantes (5 commits)

Objetivo: derrubar todos os 🔴 que quebram chamadas reais.

### Commit 1 — `fix: usar PUT em vez de POST nas atualizações`

**Arquivos a alterar:**
- [Managers/CustomerManager.cs](Codout.Apis.Asaas/Managers/CustomerManager.cs) — `Update`: trocar `PostAsync` por `PutAsync`
- [Managers/PaymentManager.cs](Codout.Apis.Asaas/Managers/PaymentManager.cs) — `Update`
- [Managers/SubscriptionManager.cs](Codout.Apis.Asaas/Managers/SubscriptionManager.cs) — `Update`, `UpdateInvoiceSettings`
- [Managers/NotificationManager.cs](Codout.Apis.Asaas/Managers/NotificationManager.cs) — `Update`, `BatchUpdate`

**Testes a adicionar:**
- Em [CustomerManagerTests.cs](Codout.Apis.Asaas.Tests/Managers/CustomerManagerTests.cs): `Update_ShouldSendPutRequest()` com `AssertRequestMethod(HttpMethod.Put)`
- Análogo em PaymentManagerTests, SubscriptionManagerTests, NotificationManagerTests
- Ajustar testes existentes que usem `Assert.Equal(HttpMethod.Post, ...)` nesses casos

**Critério de aceite:** todos os 6 métodos enviam `PUT`, todos os testes existentes continuam verdes.

---

### Commit 2 — `fix(fiscalInfo): corrigir rota para /fiscalInfo e mover ListMunicipalServices`

**Arquivos a alterar:**
- [Managers/CustomerFiscalInfoManager.cs](Codout.Apis.Asaas/Managers/CustomerFiscalInfoManager.cs) — `CustomerFiscalInfoRoute = "/fiscalInfo"` (já com `/v3` adicionado pelo BaseManager)
- [Managers/InvoiceManager.cs](Codout.Apis.Asaas/Managers/InvoiceManager.cs) — remover `ListMunicipalServices` (será movido)
- Renomear `CustomerFiscalInfoManager` → `FiscalInfoManager` (e mover `Models/CustomerFiscalInfo/` → `Models/FiscalInfo/`). Atualizar referências em `AsaasApi.cs` e em testes.
- Mover `MunicipalService` de `Models/Invoice/` → `Models/FiscalInfo/` (se o tipo for usado só nesse contexto)
- Adicionar método `ListServices(string description)` em `FiscalInfoManager` chamando `/fiscalInfo/services`

**Testes a adicionar:**
- Em [CustomerFiscalInfoManagerTests.cs](Codout.Apis.Asaas.Tests/Managers/CustomerFiscalInfoManagerTests.cs): renomear arquivo, `AssertRequestUrl("/v3/fiscalInfo/...")`
- Mover testes de `ListMunicipalServices` para o novo manager
- Em [InvoiceManagerTests.cs](Codout.Apis.Asaas.Tests/Managers/InvoiceManagerTests.cs): remover teste antigo de `ListMunicipalServices`

**Breaking change:** classe pública renomeada. Documentar no CHANGELOG.

---

### Commit 3 — `fix(finance): Balance retorna objeto { balance: decimal }`

**Arquivos a alterar:**
- Criar `Models/Finance/Balance.cs` com `[JsonPropertyName("balance")] public decimal Value { get; set; }`
- [Managers/FinanceManager.cs](Codout.Apis.Asaas/Managers/FinanceManager.cs) — assinatura: `Task<ResponseObject<Balance>> GetBalance()`

**Testes a adicionar:**
- Em [FinanceManagerTests.cs](Codout.Apis.Asaas.Tests/Managers/FinanceManagerTests.cs): `GetBalance_ShouldParseObjectResponse()` com payload `{"balance": 5210.96}` e `Assert.Equal(5210.96m, response.Data.Value)`
- Test de serialização do `Balance` em SerializationTests.cs

**Breaking change:** método renomeado `Balance()` → `GetBalance()` e tipo de retorno mudou.

---

### Commit 4 — `fix(anticipation): remover SignAgreement (endpoint inexistente)`

**Arquivos a alterar:**
- [Managers/AnticipationManager.cs](Codout.Apis.Asaas/Managers/AnticipationManager.cs) — remover `SignAgreement`
- Remover `Models/Anticipation/SignAnticipationAgreementRequest.cs`

**Testes a alterar:**
- [AnticipationManagerTests.cs](Codout.Apis.Asaas.Tests/Managers/AnticipationManagerTests.cs) — remover teste correspondente

**Nota:** Antes do commit, vou rodar uma busca via MCP do Asaas (`mcp__asaas__search`) por "agreement" para confirmar 100% que o endpoint não existe sob outro path. Se existir, criamos endpoint correto em vez de remover.

**Breaking change:** método público removido.

---

### Commit 5 — `feat(installments): adicionar Create e endpoints faltantes`

**Arquivos a criar:**
- `Models/Installment/CreateInstallmentRequest.cs` — campos básicos + sem cartão
- `Models/Installment/CreateInstallmentWithCreditCardRequest.cs` — herda + cartão
- `Models/Installment/UpdateInstallmentSplitsRequest.cs`

**Arquivos a alterar:**
- [Managers/InstallmentManager.cs](Codout.Apis.Asaas/Managers/InstallmentManager.cs) — adicionar:
  - `Create(CreateInstallmentRequest)` → POST `/installments`
  - `CreateWithCreditCard(CreateInstallmentWithCreditCardRequest)` → POST `/installments/` (com barra final — entender se o BaseManager preserva, **provavelmente não** — adaptar `BuildApiRoute`)
  - `ListPayments(string installmentId, int offset, int limit)` → GET `/installments/{id}/payments`
  - `CancelPendingPayments(string installmentId)` → DELETE `/installments/{id}/payments`
  - `UpdateSplits(string installmentId, UpdateInstallmentSplitsRequest)` → PUT `/installments/{id}/splits`

**Atenção ao detalhe da barra final:** o `BaseManager.BuildApiRoute` atualmente faz `$"/v3{(resource[0] == '/' ? string.Empty : "/")}{resource}"`. Não preserva barra final. **Vai ser preciso ajustar** ou usar uma sinalização (`AppendTrailingSlash`).

**Testes a adicionar (em InstallmentManagerTests.cs):**
- `Create_ShouldSendPostToInstallmentsRoot()`
- `CreateWithCreditCard_ShouldSendPostToInstallmentsRootWithTrailingSlash()` — valida que a URL final é `/v3/installments/` e não `/v3/installments`
- `ListPayments_ShouldSendGet()`
- `CancelPendingPayments_ShouldSendDelete()`
- `UpdateSplits_ShouldSendPut()`

---

## Sprint 2 — Reescrita do WebhookManager (1 commit grande)

### Commit 6 — `feat!(webhooks): migrar para CRUD por ID em /v3/webhooks`

**BREAKING CHANGE** — toda a API pública do `WebhookManager` muda.

**Arquivos a deletar:**
- Tudo em `Models/Webhook/` (criar do zero — Webhook.cs, WebhookRequest.cs com forma antiga)

**Arquivos a criar:**
- `Models/Webhook/Webhook.cs` — com `Id`, `Name`, `Url`, `Email`, `Enabled`, `Interrupted`, `ApiVersion`, `HasAuthToken`, `SendType`, `PenalizedRequestsCount`, `List<WebhookEvent> Events`
- `Models/Webhook/CreateWebhookRequest.cs` — campos required: `Name`, `Url`, `Email`, `Enabled`, `Interrupted`, `ApiVersion`, `AuthToken` (min 32 chars), `SendType`, `Events`
- `Models/Webhook/UpdateWebhookRequest.cs` — similar mas todos opcionais
- `Models/Webhook/WebhookListFilter.cs` — query params (validar quais a API aceita)
- `Models/Webhook/Enums/WebhookSendType.cs` — `SEQUENTIALLY`, `NON_SEQUENTIALLY`
- `Models/Webhook/Enums/WebhookEvent.cs` — **~100 valores** (extrair do OpenAPI via `mcp__asaas__get-endpoint`)

**Arquivos a alterar:**
- [Managers/WebhookManager.cs](Codout.Apis.Asaas/Managers/WebhookManager.cs) — reescrever:
  - `Create(CreateWebhookRequest)` → POST `/webhooks`
  - `List(int offset, int limit, WebhookListFilter? filter = null)` → GET `/webhooks`
  - `Find(string id)` → GET `/webhooks/{id}`
  - `Update(string id, UpdateWebhookRequest)` → PUT `/webhooks/{id}`
  - `Delete(string id)` → DELETE `/webhooks/{id}`
  - `RemoveBackoff(string id)` → POST `/webhooks/{id}/removeBackoff`

**Testes a reescrever (WebhookManagerTests.cs):**
- Remover todos os testes do padrão antigo
- Adicionar 1 teste por método novo (6 testes) + 1 teste de error
- 1 teste de serialização do enum `WebhookEvent` cobrindo pelo menos 5 valores variados

**CHANGELOG:** entrada explícita de breaking + exemplo de migração.

---

## Sprint 3 — Endpoints faltantes em managers existentes (8 commits)

### Commit 7 — `feat(payment): documentos, simulate, limits, billingInfo, viewingInfo, status, refunds, bankSlip/refund, payWithCard, captureAuthorizedPayment`

13 endpoints novos. Subdividir em sub-tópicos no mesmo commit:

**Modelos novos:**
- `Models/Payment/PaymentDocument.cs`
- `Models/Payment/CreatePaymentDocumentRequest.cs` (multipart)
- `Models/Payment/UpdatePaymentDocumentRequest.cs`
- `Models/Payment/PaymentBillingInfo.cs`
- `Models/Payment/PaymentViewingInfo.cs`
- `Models/Payment/PaymentStatusInfo.cs` (só `status`)
- `Models/Payment/SimulatePaymentRequest.cs`
- `Models/Payment/SimulatedPayment.cs`
- `Models/Payment/PaymentLimits.cs`
- `Models/Payment/PaymentRefund.cs` (para a lista)
- `Models/Payment/CapturePaymentRequest.cs`
- `Models/Payment/PayWithCreditCardRequest.cs`

**PaymentManager — adicionar métodos:**
| Método | HTTP | Rota |
|---|---|---|
| `CreateWithCreditCard(CreatePaymentRequest)` | POST | `/payments/` |
| `CaptureAuthorizedPayment(string id, CapturePaymentRequest)` | POST | `/payments/{id}/captureAuthorizedPayment` |
| `PayWithCreditCard(string id, PayWithCreditCardRequest)` | POST | `/payments/{id}/payWithCreditCard` |
| `GetBillingInfo(string id)` | GET | `/payments/{id}/billingInfo` |
| `GetViewingInfo(string id)` | GET | `/payments/{id}/viewingInfo` |
| `GetStatus(string id)` | GET | `/payments/{id}/status` |
| `Simulate(SimulatePaymentRequest)` | POST | `/payments/simulate` |
| `GetLimits()` | GET | `/payments/limits` |
| `UploadDocument(string id, AsaasFile)` | POST multipart | `/payments/{id}/documents` |
| `ListDocuments(string id, int o, int l)` | GET | `/payments/{id}/documents` |
| `FindDocument(string id, string docId)` | GET | `/payments/{id}/documents/{docId}` |
| `UpdateDocument(string id, string docId, UpdatePaymentDocumentRequest)` | PUT | `/payments/{id}/documents/{docId}` |
| `DeleteDocument(string id, string docId)` | DELETE | `/payments/{id}/documents/{docId}` |
| `ListRefunds(string id, int o, int l)` | GET | `/payments/{id}/refunds` |
| `RefundBankSlip(string id)` | POST | `/payments/{id}/bankSlip/refund` |

**Adicionar à `Payment` (response model):** `object`, `checkoutSession`, `paymentLink`, `installmentNumber`, `pixTransaction`, `pixQrCodeId`, `creditDate`, `estimatedCreditDate`, `transactionReceiptUrl`, `nossoNumero`, `anticipable`, `daysAfterDueDateToRegistrationCancellation`, `canBePaidAfterDueDate`, `chargeback`, `escrow`, `refunds`.

**Adicionar à `CreatePaymentRequest`:** `daysAfterDueDateToRegistrationCancellation`, `callback` (com `successUrl` e `autoRedirect`), `pixAutomaticAuthorizationId`.

**Adicionar à `PaymentStatus`:** `REFUND_IN_PROGRESS`.

**Testes:** 1 happy + 1 erro para cada método novo (30 testes); testes de serialização para cada novo model.

---

### Commit 8 — `feat(subscription): updateCreditCard`
- `UpdateCreditCard(string id, UpdateSubscriptionCreditCardRequest)` → PUT `/subscriptions/{id}/creditCard`

### Commit 9 — `feat(anticipation): cancel, limits, configurations`
- `Cancel(string id)` → POST `/anticipations/{id}/cancel`
- `GetLimits()` → GET `/anticipations/limits`
- `GetAutomaticConfiguration()` → GET `/anticipations/configurations`
- `UpdateAutomaticConfiguration(UpdateAnticipationConfigRequest)` → PUT `/anticipations/configurations`

### Commit 10 — `feat(creditCard): preAuthorization config`
- `SavePreAuthorizationConfig(SavePreAuthConfigRequest)` → POST `/creditCard/preAuthorization/config`
- `GetPreAuthorizationConfig()` → GET `/creditCard/preAuthorization/config`

### Commit 11 — `feat(transfer): cancel + roteamento correto para /transfers/`
- Renomear overloads `Execute` para `TransferToBankAccount` (POST `/transfers`) e `TransferToAsaasAccount` (POST `/transfers/` — preservar barra)
- `Cancel(string id)` → DELETE `/transfers/{id}/cancel`

### Commit 12 — `feat(pix): static qrCode delete, transaction find, tokenBucket`
- `DeleteStaticQrCode(string id)` → DELETE `/pix/qrCodes/static/{id}`
- `FindTransaction(string id)` → GET `/pix/transactions/{id}`
- `GetAddressKeyTokenBucket()` → GET `/pix/tokenBucket/addressKey`

### Commit 13 — `feat(myAccount): commercialInfo, status, documents`

**Breaking:** o `Find()` atual aponta para `/myAccount` mas esse endpoint na verdade é DELETE (excluir White Label). Corrigir e renomear.

- Renomear `Find()` → `GetCommercialInfo()` (GET `/myAccount/commercialInfo`)
- `UpdateCommercialInfo(UpdateCommercialInfoRequest)` → POST `/myAccount/commercialInfo`
- `GetStatus()` → GET `/myAccount/status`
- `DeleteWhiteLabelAccount()` → DELETE `/myAccount`
- `ListPendingDocuments()` → GET `/myAccount/documents`
- `SubmitDocument(string id, List<AsaasFile>)` → POST multipart `/myAccount/documents/{id}`
- `ViewDocumentFile(string id)`, `UpdateDocumentFile(string id, AsaasFile)`, `DeleteDocumentFile(string id)` → GET/POST/DELETE `/myAccount/documents/files/{id}`

### Commit 14 — `feat(asaasAccount): find, accessTokens, resendActivationLink`
- `Find(string id)` → GET `/accounts/{id}`
- `ResendActivationLink(string id)` → POST `/accounts/{id}/resendActivationLink`
- `CreateAccessToken(string id, CreateAccessTokenRequest)` → POST `/accounts/{id}/accessTokens`
- `ListAccessTokens(string id, int o, int l)` → GET `/accounts/{id}/accessTokens`
- `UpdateAccessToken(string id, string tokenId, UpdateAccessTokenRequest)` → PUT `/accounts/{id}/accessTokens/{tokenId}`
- `DeleteAccessToken(string id, string tokenId)` → DELETE `/accounts/{id}/accessTokens/{tokenId}`

**Testes:** 1 happy + 1 erro por método.

---

## Sprint 4 — Novos Domínios (6 commits)

### Commit 15 — `feat: ChargebackManager`
- Novo manager + modelos (`Chargeback`, `ChargebackDispute`, enums `ChargebackStatus`, `ChargebackReason`, `ChargebackDisputeStatus`)
- 3 endpoints: List `/chargebacks`, FindByPayment `/payments/{id}/chargeback`, CreateDispute `/chargebacks/{id}/dispute`
- Adicionar `Lazy<ChargebackManager>` em [AsaasApi.cs](Codout.Apis.Asaas/AsaasApi.cs)
- `Testable` + ManagerTests file
- Testes: 3 happy + 3 erro + serialização

### Commit 16 — `feat: EscrowManager`
- 5 endpoints: salvar/recuperar config por subconta, default, finish, recuperar de payment
- Modelos: `EscrowConfig`, `Escrow`

### Commit 17 — `feat: CheckoutManager`
- 2 endpoints + modelos `Checkout`, `CreateCheckoutRequest`

### Commit 18 — `feat: MobilePhoneRechargeManager`
- 4 endpoints + modelos: `MobilePhoneRecharge`, `CreateMobilePhoneRechargeRequest`, `MobilePhoneProvider`

### Commit 19 — `feat(payment): splits queries`
- Adicionar em `PaymentManager`:
  - `ListPaidSplits(int o, int l)`, `FindPaidSplit(string id)`
  - `ListReceivedSplits(int o, int l)`, `FindReceivedSplit(string id)`
- Modelo `PaidSplit` / `ReceivedSplit`

### Commit 20 — `feat: SandboxManager`
- 3 endpoints sandbox-only: ApproveAccount, ConfirmPayment, ForceOverdue
- **Adicionar guarda:** método lança `InvalidOperationException` se `apiSettings.AsaasEnvironment.IsProduction()`

---

## Sprint 5 — Pix Evolução (2 commits)

### Commit 21 — `feat: PixAutomaticManager`
- 6 endpoints sob `/pix/automatic/*`
- Modelos: `PixAutomaticAuthorization`, `PixAutomaticPaymentInstruction`, enums

### Commit 22 — `feat: PixRecurringManager`
- 5 endpoints sob `/pix/transactions/recurrings/*`
- Modelos: `PixRecurringTransaction`, `PixRecurringItem`

---

## Sprint 6 — Polimento e Quality (4 commits)

### Commit 23 — `refactor(models): completar campos faltantes em response DTOs`
- Customer: `object`, `cityName`, `foreignCustomer`, `stateInscription`, `groupName`, `company`
- CreateCustomerRequest/UpdateCustomerRequest: `company`, `foreignCustomer` (e `groupName` no Update)
- Demais ajustes pontuais identificados na auditoria
- Testes de serialização para cada campo adicionado

### Commit 24 — `refactor(models): bool? em campos opcionais, revisar DateTime`
- Converter `bool` → `bool?` em campos não-required de responses (Customer.Deleted, Customer.NotificationDisabled, Payment.PostalService, Payment.Anticipated, Payment.Deleted, etc.)
- Adicionar conversor `JsonConverter<DateOnly>` (já que .NET 10) para campos `format: date` se necessário
- Atualizar testes para cobrir caso nulo

### Commit 25 — `refactor(core): usar IHttpClientFactory`
- Adicionar `AddAsaasClient` em `Codout.Apis.Asaas/Extensions/ServiceCollectionExtensions.cs` (nova)
- Refatorar `BaseManager` para receber `IHttpClientFactory` ao invés de criar `new HttpClient()` por requisição
- Manter `ApiSettings`/`AsaasApi(ApiSettings)` para compatibilidade não-DI
- **Atenção:** ajustar `TestableManagerFactory` (testes que sobrescrevem `BuildHttpClient`)

### Commit 26 — `chore: WasSuccessful (manter WasSucessfull como alias temporário)`
- Adicionar `WasSuccessful()` correto em `BaseResponse`
- Manter `WasSucessfull()` chamando o novo, marcado `[Obsolete("Use WasSuccessful (typo correction).")]`
- Atualizar uso interno

---

## Commit final — `chore(release): bump version to 3.0.0 + CHANGELOG`

**Arquivos a alterar:**
- [Codout.Apis.Asaas.csproj](Codout.Apis.Asaas/Codout.Apis.Asaas.csproj) — `<Version>3.0.0</Version>`
- [CHANGELOG.md](CHANGELOG.md) — entrada `## 3.0.0 — 2026-MM-DD`
  - Seção "Breaking changes" listando todas as renomeações e remoções
  - Seção "Bug fixes" com os 7 fixes
  - Seção "New endpoints" agrupados por manager
  - Seção "New managers" (Chargeback, Escrow, Checkout, MobilePhoneRecharge, Sandbox, PixAutomatic, PixRecurring)
  - Guia de migração 2.x → 3.x

---

## Verificação Final (antes de fazer push)

```powershell
dotnet build Codout.Apis.Asaas/ --configuration Release
dotnet test Codout.Apis.Asaas.Tests/ --configuration Release
dotnet pack Codout.Apis.Asaas/ --configuration Release --no-build
```

Critérios:
- Build sem warnings novos (exceto os ignorados em `NoWarn`)
- **Todos os testes passando** (existentes + novos — deve fechar com ~600+ testes)
- Pacote NuGet gerado com sucesso
- AUDIT.md ainda no repo como histórico — depois de released, mover para `docs/`

---

## Métricas-alvo desta entrega

| Métrica | Antes | Depois |
|---|---|---|
| Endpoints SDK | ~70 | ~156 (100%) |
| Managers | 20 | 27 (+7 novos) |
| Testes | ~400 | ~600+ |
| Bugs bloqueantes conhecidos | 7 | 0 |
| Versão | 2.0.2 | 3.0.0 |

---

## Ordem de execução nesta sessão

Vou executar sprint a sprint, **fazendo um commit por vez**, sempre após:
1. Implementar a mudança
2. Adicionar/ajustar testes
3. Rodar `dotnet build` + `dotnet test` localmente
4. Só então `git add` + `git commit`

Se algum commit falhar build/testes, **paro e te aviso** antes de prosseguir. Você pode interromper a qualquer momento e me dizer "pula esse" ou "ajusta isso".

A cada fim de Sprint farei uma pausa pra você revisar o que foi feito, antes de continuar para o próximo.
