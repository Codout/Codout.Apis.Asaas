# Code Review — branch `audit/asaas-api-conformance` (v3.0.0)

> Revisado em: 2026-05-24
> Fontes: (1) subagent code-reviewer (qualidade de código + testes), (2) verificação manual de schemas contra o MCP oficial do Asaas
> Commits revisados: 28 (master..HEAD)

---

## Sumário executivo

O refactor entrega o objetivo macro: cobertura sobe de ~45% para 100% dos endpoints documentados, 7 bugs bloqueantes originais corrigidos, 7 novos managers, 478 testes passando. **MAS** vários modelos novos foram criados sem leitura cuidadosa do schema OpenAPI — vou rebatizar como "API conformance bugs" os casos em que o cliente vai falhar contra a API real apesar dos testes locais passarem (porque testes mockam JSON).

Encontrei **8 🔴 bloqueantes** (1 do subagent, 7 de schema), **9 🟡 importantes**, **3 🟢 polimento**.

---

## 🔴 BLOQUEANTES

### B-01 — `PostMultipartFormDataContentAsync`: NRE em propriedades nulas
**Fonte:** subagent | **Arquivo:** [Core/BaseManager.cs:61](Codout.Apis.Asaas/Core/BaseManager.cs#L61)

```csharp
multipartContent.Add(new StringContent(prop.GetValue(payload).ToString()), jsonPropertyName);
```

Crash com `NullReferenceException` quando qualquer propriedade não-arquivo do payload é `null`. Novos call sites introduzidos neste PR que expõem o bug:
- `PaymentManager.UploadDocument` (UploadPaymentDocumentRequest.Available é `bool?`, garantido de ser null em uploads sem esse parâmetro)
- `PaymentManager.UploadDocument` (UploadPaymentDocumentRequest.Type — string)
- `ChargebackManager.CreateDispute` (CreateChargebackDisputeRequest.Description — string)

**Fix:** ignorar propriedades nulas no loop de reflection.

---

### B-02 — Modelo `PaymentLimits` tem shape errado (deserialização vai virar `null`/0)
**Fonte:** schema review | **Arquivo:** [Models/Payment/PaymentLimits.cs](Codout.Apis.Asaas/Models/Payment/PaymentLimits.cs)

API real (`/v3/payments/limits`):
```json
{
  "creation": {
    "daily": { "limit": 10, "used": 5, "wasReached": false }
  }
}
```

Meu modelo tinha `CreditCard`/`Pix`/`BankSlip` cada um com `Daily`/`Monthly`/`AverageTicket` — **completamente diferente**.

**Fix:** reescrever.

---

### B-03 — `SimulatePaymentRequest` e `SimulatedPayment` com shape errado
**Fonte:** schema review | **Arquivo:** [Models/Payment/SimulatePaymentRequest.cs](Codout.Apis.Asaas/Models/Payment/SimulatePaymentRequest.cs), [Models/Payment/SimulatedPayment.cs](Codout.Apis.Asaas/Models/Payment/SimulatedPayment.cs)

Request real exige `value` + `billingTypes` (lista, plural) + opcional `installmentCount`. Meu modelo tinha `BillingType` singular, mais campos inventados (`DiscountValue`, `Splits`).

Response real é `{ value, creditCard: {...}, bankSlip: {...}, pix: {...} }`. Meu modelo era flat com `NetValue`, `Fee`, `InstallmentValue`.

API vai rejeitar requests e responses não vão deserializar.

**Fix:** reescrever ambos.

---

### B-04 — `Checkout` / `CreateCheckoutRequest` com shape muito errado
**Fonte:** schema review | **Arquivo:** [Models/Checkout/Checkout.cs](Codout.Apis.Asaas/Models/Checkout/Checkout.cs)

API real:
- Response (`CheckoutSessionResponseDTO`) tem `id`, `link` (não `checkoutUrl`), `status`, `billingTypes`, `chargeTypes`, `minutesToExpire`, `externalReference`, `callback`, `items`, `customerData`, `subscription`, `installment`, `split`
- Request (`CheckoutSessionSaveRequestDTO`) tem campos **obrigatórios** `billingTypes`, `chargeTypes`, `callback`, `items` — meu modelo nem tinha `items`!
- `CheckoutCustomerData.City` é `int` (código IBGE), não string
- `CheckoutCustomerData.AddressNumber` é `int` (?), não string
- `CheckoutCallback` tem `cancelUrl` obrigatório e `expiredUrl` opcional

Meu modelo tinha `Value`, `DueDate`, `Customer`, `CustomerData` — vários inventados, vários ausentes.

**Status enum** correto: `ACTIVE, CANCELED, EXPIRED, PAID`.

**Fix:** reescrita substancial.

---

### B-05 — `MobilePhoneRecharge` tem campo errado e enum como string
**Fonte:** schema review | **Arquivo:** [Models/MobilePhoneRecharge/MobilePhoneRecharge.cs](Codout.Apis.Asaas/Models/MobilePhoneRecharge/MobilePhoneRecharge.cs)

API real:
- `operatorName` (não `provider`)
- `canBeCancelled` (faltando)
- Status é enum: `PENDING, CONFIRMED, CANCELLED, REFUNDED, WAITING_CRITICAL_ACTION`
- Não tem `DateCreated` nem `ConfirmedDate`

**Fix:** ajustar nome do campo, adicionar `canBeCancelled`, converter status para enum.

---

### B-06 — `PixAutomaticAuthorization` totalmente diferente do schema da API
**Fonte:** schema review | **Arquivo:** [Models/PixAutomatic/PixAutomaticAuthorization.cs](Codout.Apis.Asaas/Models/PixAutomatic/PixAutomaticAuthorization.cs)

API real:
- Response: `id, minLimitValue, cancellationDate, cancellationReason, contractId, customerId, description, finishDate, frequency` (não `periodicity`), `endToEndIdentifier, startDate, status, value, payload, encodedImage, immediateQrCode, originType, subscriptionId`
- Request: requer `frequency, contractId, startDate, customerId, immediateQrCode` (objeto complexo!) — `immediateQrCode` é totalmente novo e obrigatório

Meu modelo tinha campos inventados (`PayerCpfCnpj`, `PayerName`, `ApprovalDate`) e faltava o `immediateQrCode` (obrigatório). API vai rejeitar todo request criado pelo SDK.

**Enums a criar:**
- `PixAutomaticRecurringFrequency`: WEEKLY, MONTHLY, QUARTERLY, SEMIANNUALLY, ANNUALLY
- `PixAutomaticAuthorizationStatus`: CREATED, ACTIVE, CANCELLED, REFUSED, EXPIRED
- `PixAutomaticRecurringOriginType`: IMMEDIATE_PAYMENT_AND_RECURRING_QR_CODE, PAYMENT_AND_RECURRING_OFFER_QR_CODE
- `PixAutomaticRecurringPaymentCreationMode`: MANUAL, SUBSCRIPTION

**Fix:** reescrita completa.

---

### B-07 — `AccountDocument` envelope errado em `ListPendingDocuments`
**Fonte:** schema review | **Arquivo:** [Managers/MyAccountManager.cs:62](Codout.Apis.Asaas/Managers/MyAccountManager.cs#L62)

API real para `GET /v3/myAccount/documents` retorna `{ rejectReasons, data: [...] }` — NÃO é o envelope `{hasMore, totalCount, limit, offset, data}` que `ResponseList<T>` espera. Meu método chama `GetListAsync<AccountDocumentSection>` que vai deserializar `rejectReasons` como ignorado mas `hasMore/totalCount/limit/offset` virão `null`/0.

Mais crítico: o shape de `AccountDocumentSection` está errado também — falta `type`, `responsible`, `onboardingUrl`, `onboardingUrlExpirationDate`; `Documents` interno tem só `id, status` (não os campos que coloquei).

**Fix:** criar `AccountDocumentResponse` wrapper e mudar retorno para `ResponseObject<AccountDocumentResponse>`. Reescrever `AccountDocument` model.

---

### B-08 — `PaymentBillingInfo.Nossonumero` grafia errada + tipos errados
**Fonte:** schema review + subagent I-06 | **Arquivo:** [Models/Payment/PaymentBillingInfo.cs](Codout.Apis.Asaas/Models/Payment/PaymentBillingInfo.cs)

- `Nossonumero` → deveria ser `NossoNumero` (camelCase consistente)
- Faltam: `bankSlipUrl`, `daysAfterDueDateToRegistrationCancellation` no `BankSlip`
- Faltam: `description` no `Pix`
- `CreditCard` na API usa `creditCardNumber/creditCardBrand/creditCardToken` (igual ao `CreditCardTokenizeResponseDTO`) — meu modelo `PaymentBillingInfoCreditCard` está OK, só rever nomes

**Fix:** ajustes pontuais nos sub-objetos.

---

## 🟡 IMPORTANTES

### I-01 — `SandboxManager._settings` duplica `BaseManager._settings`
**Fonte:** subagent | **Arquivo:** [Managers/SandboxManager.cs:12](Codout.Apis.Asaas/Managers/SandboxManager.cs#L12)

Campo redundante. Fix: tornar `_settings` em `BaseManager` como `protected` e remover do SandboxManager. Não causa bug hoje, mas é confuso.

### I-02 — ~120 chamadas a `WasSucessfull()` `[Obsolete]` em testes gerando warnings CS0618
**Fonte:** subagent | **Arquivos:** todos `*ManagerTests.cs`, `ResponseObjectTests.cs`, `ResponseListTests.cs`, `Sample/Program.cs`

CHANGELOG diz que migrou para `WasSuccessful()`, mas testes ainda usam grafia antiga. Polui output de CI. **Fix:** replace global.

### I-03 — `SerializationTests` usa `JsonStringEnumConverter` (padrão) em vez de `SafeEnumConverterFactory` (do SDK)
**Fonte:** subagent | **Arquivo:** [Tests/Models/SerializationTests.cs:20-32](Codout.Apis.Asaas.Tests/Models/SerializationTests.cs#L20-L32)

Testes não exercitam os conversores customizados. Um enum desconhecido passaria no teste e quebraria em produção.

### I-04 — `Installment.Deleted` é `bool` (não `bool?`) — inconsistente com migração 3.0.0
**Fonte:** subagent | **Arquivo:** [Models/Installment/Installment.cs:46](Codout.Apis.Asaas/Models/Installment/Installment.cs#L46)

Customer.Deleted e Payment.Deleted foram convertidos; Installment ficou de fora.

### I-05 — `SandboxManager`: só `ApproveAccount` tem teste de guarda de produção
**Fonte:** subagent | **Arquivo:** [Tests/Managers/SandboxManagerTests.cs](Codout.Apis.Asaas.Tests/Managers/SandboxManagerTests.cs)

`ConfirmPayment_InProduction_Throws` e `ForceOverdue_InProduction_Throws` faltando.

### I-06 — `AsaasApi.ReceivableAnticipation` vs todos os outros managers sem prefixo
**Fonte:** subagent | **Arquivo:** [AsaasApi.cs:50](Codout.Apis.Asaas/AsaasApi.cs#L50)

Inconsistência pré-existente (não introduzida neste PR), mas vale ajustar para `Anticipation` num major.

### I-07 — Múltiplas classes por arquivo em modelos novos
**Fonte:** subagent | **Arquivos:** Chargeback.cs, Escrow.cs, Checkout.cs, AccessToken.cs, PixAutomaticAuthorization.cs, PixRecurringTransaction.cs, MobilePhoneRecharge.cs, PaymentBillingInfo.cs, PaymentLimits.cs, AccountDocument.cs

Viola convenção pré-existente "1 classe por arquivo". Dificulta navegação no editor.

### I-08 — Testes de erro ausentes em 5 managers novos
**Fonte:** subagent | **Arquivos:** CheckoutManagerTests.cs, EscrowManagerTests.cs, PixAutomaticManagerTests.cs, PixRecurringManagerTests.cs, MobilePhoneRechargeManagerTests.cs

Plano dizia "1 happy + 1 erro por endpoint" — nenhum desses tem teste de erro.

### I-09 — `AccountStatus` deveria usar enum em vez de string
**Fonte:** schema review | **Arquivo:** [Models/MyAccount/AccountStatus.cs](Codout.Apis.Asaas/Models/MyAccount/AccountStatus.cs)

API documenta enum `PENDING|APPROVED|REJECTED|AWAITING_APPROVAL` para todos os campos de status. Strings funcionam (`SafeEnumConverterFactory` cobre default), mas perde type safety.

---

## 🟢 POLIMENTO

### P-01 — `SerializationTests.cs` namespace inconsistente com diretório
**Fonte:** subagent | Em `Tests/Models/` mas namespace é `Tests.Serialization`.

### P-02 — Estilo de namespace inconsistente em `Models/AsaasAccount/`
**Fonte:** subagent | `Account.cs` usa bloco `{}`, `AccessToken.cs` usa file-scoped `;`.

### P-03 — Cast `(object)` em `EscrowManager.FinishPaymentEscrow` precisa de comentário
**Fonte:** subagent | Necessário pra resolver `??` mas não-óbvio.

---

## Verificações que NÃO são bugs (já confirmadas)

| Verificação | Resultado |
|---|---|
| `BuildApiRoute` preserva trailing slash? | ✅ Sim |
| `disposeHandler: false` + `using` é seguro? | ✅ Sim |
| `(object)requestObj ?? new RequestParameters()` resolve corretamente? | ✅ Sim |
| Todos os 27 managers expostos em `AsaasApi.cs`? | ✅ Sim |
| Todos têm `Testable*Manager`? | ✅ Sim |
| `WasSucessfull` chamado internamente no SDK fora de testes? | ✅ Não |
| Endpoint `/v3/customers` schema (Customer, Create, Update) | ✅ Sim (verificado nos commits 1+23) |
| `/v3/finance/balance` shape | ✅ Sim (verificado no commit 3) |
| `/v3/webhooks` CRUD | ✅ Sim (verificado no commit 6) |
| `/v3/fiscalInfo/services` | ✅ Sim (verificado no commit 2) |
| `/v3/myAccount/commercialInfo` | ✅ Sim (verificado no commit 13) |
| Notas fiscais (Invoice) | ⚠️ Não verificado a fundo, modelo é pré-existente |

---

## Plano de ação (recomendado)

**Imediato (este sessão):**
1. B-01 — fix NRE no multipart
2. B-08 — fix `Nossonumero` typo e completar BankSlip/Pix
3. B-05 — fix MobilePhoneRecharge (provider→operatorName, canBeCancelled, status enum)
4. B-02 — rewrite PaymentLimits
5. B-03 — rewrite SimulatePayment + SimulatedPayment
6. B-04 — rewrite Checkout/CreateCheckoutRequest
7. B-06 — rewrite PixAutomaticAuthorization (mais trabalhoso)
8. B-07 — rewrite AccountDocument + envelope
9. I-04 — Installment.Deleted bool?
10. I-02 — global replace WasSucessfull → WasSuccessful

**Pós-revisão (próximo PR / no merge):**
- I-01, I-05, I-06, I-07, I-08, I-09 — qualidade
- P-01, P-02, P-03 — polimento
