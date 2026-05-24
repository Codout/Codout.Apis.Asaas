# Auditoria de Conformidade — SDK Codout.Apis.Asaas vs API Oficial Asaas

> **Gerado em:** 2026-05-24
> **Fonte da verdade:** MCP `https://docs.asaas.com/mcp` (OpenAPI 3.0.1)
> **Versão do SDK auditada:** 2.0.3 (commit `d23bbb7`)
> **Cobertura da auditoria:** 20 managers, 156 endpoints documentados

---

## 1. Sumário Executivo

A API Asaas v3 documenta **~156 endpoints** distribuídos em **~25 domínios funcionais**. O SDK atualmente implementa cerca de **70 endpoints** em **20 managers** — cobrindo **~45% da superfície da API**.

A auditoria identificou **6 bugs críticos** (categoria 🔴 — quebram chamadas em produção), **dezenas de endpoints faltando** em managers existentes, e **8 domínios inteiros não cobertos**.

### Distribuição de problemas

| Severidade | Quantidade | Descrição |
|---|---|---|
| 🔴 Bloqueante | **6 bugs + 4 domínios inteiros errados** | Quebram integrações reais |
| 🟡 Importante | ~50 endpoints faltando + ~30 campos faltando | Funcionalidade limitada |
| 🟢 Cosmético | ~10 pontos | Polimento (typos, naming) |

### Top 6 prioridades absolutas (a tratar primeiro)

1. **`PostAsync` usado onde a API exige `PUT`** em 6 métodos de Update (Customer, Payment, Subscription × 2, Notification × 2). Todas as atualizações estão sendo enviadas com HTTP errado.
2. **`WebhookManager` inteiro está num modelo de API antigo** (`/webhook` singular, paths `/webhook/invoice` e `/webhook/mobilePhoneRecharge` que **não existem** mais). Precisa reescrita completa para o novo padrão CRUD em `/v3/webhooks/{id}`.
3. **`CustomerFiscalInfoManager` usa rota errada**: `/customerFiscalInfo` mas a API expõe `/fiscalInfo/`. Todas as chamadas retornam 404.
4. **`AnticipationManager.SignAgreement`** chama `/anticipations/agreement/sign` — endpoint **inexistente** na documentação atual.
5. **`InstallmentManager` não tem método `Create`**, sendo um manager de parcelamento que não consegue criar parcelamentos. Bug por omissão de funcionalidade fundamental.
6. **`FinanceManager.Balance()`** retorna `decimal` direto, mas a API retorna `{ balance: number }`. A deserialização falha silenciosamente.

---

## 2. Bugs Críticos (🔴 Bloqueantes)

### 🔴 BUG-001 — `PostAsync` em vez de `PutAsync` em métodos de Update

A API documenta `PUT` para os endpoints abaixo, mas o SDK envia `POST`:

| Manager | Método | Endpoint atual no SDK | Endpoint correto |
|---|---|---|---|
| `CustomerManager` | `Update` (linha 36) | POST `/v3/customers/{id}` | **PUT** `/v3/customers/{id}` |
| `PaymentManager` | `Update` (linha 36) | POST `/v3/payments/{id}` | **PUT** `/v3/payments/{id}` |
| `SubscriptionManager` | `Update` (linha 38) | POST `/v3/subscriptions/{id}` | **PUT** `/v3/subscriptions/{id}` |
| `SubscriptionManager` | `UpdateInvoiceSettings` (linha 82) | POST `/v3/subscriptions/{id}/invoiceSettings` | **PUT** `/v3/subscriptions/{id}/invoiceSettings` |
| `NotificationManager` | `Update` (linha 15) | POST `/v3/notifications/{id}` | **PUT** `/v3/notifications/{id}` |
| `NotificationManager` | `BatchUpdate` (linha 21) | POST `/v3/notifications/batch` | **PUT** `/v3/notifications/batch` |

**Por que escapou aos testes:** Os mocks aceitam qualquer método HTTP. Recomenda-se reforçar `AssertRequestMethod` em todos os testes de update.

**Bom:** `PaymentLinkManager.Update` e `InvoiceManager.Update` já usam `PutAsync` corretamente.

### 🔴 BUG-002 — `WebhookManager` num modelo de API obsoleto

O manager atual implementa um padrão antigo de "webhook único por tipo" com paths inexistentes:

```csharp
// SDK atual (TODOS errados):
"/webhook"                      → não existe
"/webhook/invoice"              → não existe
"/webhook/mobilePhoneRecharge"  → não existe
```

A API atual expõe **CRUD genérico por ID** em `/v3/webhooks`:

```
POST   /v3/webhooks                    Create
GET    /v3/webhooks                    List
GET    /v3/webhooks/{id}               Find
PUT    /v3/webhooks/{id}               Update
DELETE /v3/webhooks/{id}               Delete
POST   /v3/webhooks/{id}/removeBackoff Remove backoff
```

Os modelos `Webhook` e `WebhookRequest` também estão incompletos. Faltam:
- `Id`, `Name`, `HasAuthToken`, `SendType` (enum: `SEQUENTIALLY` / `NON_SEQUENTIALLY`)
- `PenalizedRequestsCount` (resposta)
- `Events` (lista de enum `WebhookEvent` com **~100 valores possíveis**)

**Impacto:** webhooks simplesmente não funcionam pelo SDK hoje.

### 🔴 BUG-003 — `CustomerFiscalInfoManager` na rota errada

```csharp
// CustomerFiscalInfoManager.cs:10
private const string CustomerFiscalInfoRoute = "/customerFiscalInfo";
```

Endpoint correto na API: **`/v3/fiscalInfo/`**.

Todas as 3 operações (`CreateOrUpdate`, `Find`, `ListMunicipalOptions`) chamam URL inexistente. Devem retornar 404.

Adicionalmente, o método `ListMunicipalOptions` faz GET em `/customerFiscalInfo/municipalOptions`, mas o endpoint correto é `/v3/fiscalInfo/municipalOptions`.

### 🔴 BUG-004 — `AnticipationManager.SignAgreement` chama endpoint inexistente

```csharp
// AnticipationManager.cs:41
var route = $"{AnticipationsRoute}/agreement/sign"; // → /v3/anticipations/agreement/sign
```

Esse endpoint **não consta na API documentada**. Verificar: pode ter sido removido ou ter mudado de path. Confirmar com a Asaas se ainda existe; se não, remover o método.

### 🔴 BUG-005 — `InstallmentManager` sem método `Create`

`InstallmentManager` expõe apenas `Find`, `List`, `Delete`, `Refund`, `ListPaymentBook`. **Falta o método mais importante**: criar parcelamento. A API documenta:

- POST `/v3/installments` — Create installment (boleto/PIX)
- POST `/v3/installments/` — Create installment with credit card (path com barra final = endpoint separado)

Adicionalmente, faltam:
- DELETE `/v3/installments/{id}/payments` — cancelar cobranças pendentes
- GET `/v3/installments/{id}/payments` — listar cobranças do parcelamento
- PUT `/v3/installments/{id}/splits` — atualizar splits

### 🔴 BUG-006 — `FinanceManager.Balance()` shape de resposta errado

```csharp
// FinanceManager.cs:13-18
public async Task<ResponseObject<decimal>> Balance()
{
    return await GetAsync<decimal>(route);
}
```

A API retorna `{ "balance": 5210.96 }` (objeto), não um `decimal` puro. `System.Text.Json` não consegue deserializar um objeto JSON em `decimal`, então `ResponseObject<decimal>.Data` virá `0` silenciosamente.

**Fix:** criar tipo `Balance { public decimal Value { get; set; } }` (com `[JsonPropertyName("balance")]`).

### 🔴 BUG-007 — `InvoiceManager.ListMunicipalServices` na rota errada

```csharp
var route = $"{InvoicesRoute}/municipalServices"; // → /v3/invoices/municipalServices
```

Endpoint correto: **`/v3/fiscalInfo/services`**. Esse método pertence ao domínio `fiscalInfo`, não a `invoices`. Deve ser movido para `CustomerFiscalInfoManager` (após corrigir BUG-003).

---

## 3. Endpoints Faltantes por Manager (🟡 Importante)

### 3.1 PaymentManager — 13 endpoints faltando
| Método | Path | Descrição |
|---|---|---|
| POST | `/v3/payments/` (com barra final) | Criar cobrança **com cartão de crédito** (endpoint separado) |
| POST | `/v3/payments/{id}/captureAuthorizedPayment` | Capturar pré-autorização |
| POST | `/v3/payments/{id}/payWithCreditCard` | Pagar cobrança com cartão |
| GET | `/v3/payments/{id}/billingInfo` | Billing info |
| GET | `/v3/payments/{id}/viewingInfo` | Viewing info |
| GET | `/v3/payments/{id}/status` | Status só |
| POST | `/v3/payments/simulate` | Simulador de vendas |
| GET | `/v3/payments/limits` | Limites |
| POST/GET | `/v3/payments/{id}/documents` | Upload / lista documentos |
| PUT/GET/DELETE | `/v3/payments/{id}/documents/{documentId}` | CRUD por documento |
| GET | `/v3/payments/{id}/refunds` | Listar estornos (diferente de POST refund) |
| POST | `/v3/payments/{id}/bankSlip/refund` | Estornar boleto |

**Campos faltando em `Payment` (response):** `object`, `checkoutSession`, `paymentLink`, `installmentNumber`, `pixTransaction`, `pixQrCodeId`, `creditDate`, `estimatedCreditDate`, `transactionReceiptUrl`, `nossoNumero`, `anticipable`, `daysAfterDueDateToRegistrationCancellation`, `canBePaidAfterDueDate`, `chargeback`, `escrow`, `refunds`.

**Campos faltando em `CreatePaymentRequest`:** `daysAfterDueDateToRegistrationCancellation`, `callback` (objeto com `successUrl` e `autoRedirect`), `pixAutomaticAuthorizationId`.

**Enum `PaymentStatus` desatualizado:** falta `REFUND_IN_PROGRESS`.

**Enum `BillingType`:** SDK provavelmente tem `BOLETO/CREDIT_CARD/PIX/...` — a API documenta também `UNDEFINED`, `DEBIT_CARD`, `TRANSFER`, `DEPOSIT` em respostas (não confirmado, validar).

### 3.2 SubscriptionManager — 1 faltando
| Método | Path |
|---|---|
| PUT | `/v3/subscriptions/{id}/creditCard` — Atualizar cartão sem cobrar |

### 3.3 PixManager — muitos faltando (PIX evolução recente)
| Método | Path |
|---|---|
| DELETE | `/v3/pix/qrCodes/static/{id}` |
| GET | `/v3/pix/tokenBucket/addressKey` |
| GET | `/v3/pix/transactions/{id}` (Find — só List existe!) |
| **Domínio PIX Automático** | POST/GET/DELETE `/v3/pix/automatic/authorizations[/{id}]`; GET `/v3/pix/automatic/paymentInstructions[/{id}]` |
| **Domínio PIX Recorrente** | GET `/v3/pix/transactions/recurrings[/{id}]`; POST `/v3/pix/transactions/recurrings/{id}/cancel`; GET `/v3/pix/transactions/recurrings/{id}/items`; POST `/v3/pix/transactions/recurrings/items/{id}/cancel` |

### 3.4 TransferManager
| Método | Path |
|---|---|
| DELETE | `/v3/transfers/{id}/cancel` |
| POST | `/v3/transfers/` (path com barra = transferência **para conta Asaas**, diferente da raiz) |

> **Observação:** o SDK tem dois overloads de `Execute()` mas ambos postam em `/transfers`. A API separa: `/transfers` (instituição externa/PIX) vs `/transfers/` (Asaas). Precisa rotear corretamente.

### 3.5 AnticipationManager
| Método | Path |
|---|---|
| POST | `/v3/anticipations/{id}/cancel` |
| GET | `/v3/anticipations/limits` |
| PUT/GET | `/v3/anticipations/configurations` — antecipação automática |

### 3.6 CreditCardManager — só tem tokenização, faltam:
| Método | Path |
|---|---|
| POST/GET | `/v3/creditCard/preAuthorization/config` |

### 3.7 MyAccountManager
| Método | Path | Obs |
|---|---|---|
| GET/POST | `/v3/myAccount/commercialInfo/` | SDK tem `Find()` apontando para `/myAccount`, mas o endpoint real é `/myAccount/commercialInfo/` |
| GET | `/v3/myAccount/status/` | Status cadastral |
| DELETE | `/v3/myAccount/` | Excluir subconta White Label (existente no SDK? **não está**) |
| GET | `/v3/myAccount/documents` | Documentos pendentes |
| POST | `/v3/myAccount/documents/{id}` | Enviar documento |
| GET/POST/DELETE | `/v3/myAccount/documents/files/{id}` | CRUD arquivo enviado |

### 3.8 AsaasAccountManager
| Método | Path |
|---|---|
| GET | `/v3/accounts/{id}` — Find by id |
| POST | `/v3/accounts/{id}/resendActivationLink` |
| POST/GET | `/v3/accounts/{id}/accessTokens` |
| PUT/DELETE | `/v3/accounts/{id}/accessTokens/{accessTokenId}` |

### 3.9 BillPaymentManager
- `List` não aceita filtro; a doc oficial tem query params para data e status (validar).

### 3.10 CreditBureauReportManager — OK estruturalmente

### 3.11 PaymentLinkManager — **completo** (todos os 9 endpoints presentes; único manager 100% conforme)

---

## 4. Domínios Inteiros Não Cobertos (🔴 Funcionalidade ausente)

| Domínio | Endpoints | Onde deveria ficar |
|---|---|---|
| **Chargebacks** | 3 endpoints (`/v3/chargebacks`, `/v3/chargebacks/{id}/dispute`, `/v3/payments/{id}/chargeback`) | Novo `ChargebackManager` |
| **Escrow / Conta de Garantia** | 5 endpoints (`/v3/accounts/{id}/escrow`, `/v3/accounts/escrow`, `/v3/escrow/{id}/finish`, `/v3/payments/{id}/escrow`) | Novo `EscrowManager` |
| **Checkouts** | 2 endpoints (`/v3/checkouts`, `/v3/checkouts/{id}/cancel`) | Novo `CheckoutManager` |
| **Mobile Phone Recharges** | 4 endpoints (`/v3/mobilePhoneRecharges/*`) | Novo `MobilePhoneRechargeManager` |
| **Payment Splits queries** | 4 endpoints (`/v3/payments/splits/{paid,received}[/{id}]`) | Métodos novos em `PaymentManager` |
| **Lean Payments** | 11 endpoints (variante "lean" de `/v3/payments`) | Opcional — pode ser parâmetro/sobrecarga no `PaymentManager` |
| **Sandbox helpers** | 3 endpoints (`/v3/sandbox/myAccount/approve`, `/v3/sandbox/payment/{id}/confirm`, `/v3/sandbox/payment/{id}/overdue`) | Novo `SandboxManager` — útil para testes E2E |
| **Customer Notifications (GET)** | `/v3/customers/{id}/notifications` | Método novo em `CustomerManager` ou `NotificationManager` |

---

## 5. Outros Achados (🟢 / 🟡)

### 5.1 `CreateCustomerRequest` / `UpdateCustomerRequest`
Faltam: `company`, `foreignCustomer`. Adicionalmente, `groupName` está só no Create — a API aceita no Update também.

### 5.2 `Customer` (response)
Faltam: `object`, `cityName`, `foreignCustomer`, `stateInscription`, `groupName`.

### 5.3 `Notification` (response)
A doc retorna mais campos por evento (tipos de notificação, `deleted`, `event`). Validar via `get-endpoint`.

### 5.4 `bool` vs `bool?` em respostas
Muitos modelos usam `bool` non-nullable para campos que a API pode omitir. Recomenda-se padronizar para `bool?` em respostas opcionais (Customer.Deleted, Customer.NotificationDisabled, Payment.PostalService, Payment.Anticipated, Payment.Deleted).

### 5.5 `DateTime` para campos formato `date` (sem hora)
A API documenta vários campos como `format: date` (`YYYY-MM-DD`), não `date-time`. `System.Text.Json` aceita ambos, mas pode causar parsing inconsistente em serialização de volta. Considerar conversor customizado ou tipo `DateOnly`.

### 5.6 `BaseManager.BuildHttpClient` cria novo `HttpClient` por requisição
Anti-pattern em .NET — risco de `SocketException` por esgotamento de portas. Migrar para `IHttpClientFactory`. **Não é divergência da doc**, mas é um problema técnico que vale corrigir junto.

### 5.7 Typo `WasSucessfull` (em vez de `WasSuccessful`)
Documentado no CLAUDE.md como conhecido. Manter por compatibilidade ou marcar como `[Obsolete]` e adicionar correto.

### 5.8 Servidor de produção
A API documentada lista `https://api-sandbox.asaas.com` como server no OpenAPI. O SDK tem `https://api.asaas.com` para produção — **isso está correto**, é apenas o exemplo do OpenAPI que usa sandbox.

---

## 6. Plano de Correção Priorizado

### Sprint 1 — Bugs bloqueantes (PRs pequenos, alto valor)

| # | PR | O quê | Impacto |
|---|---|---|---|
| 1 | `fix: usar PUT para atualizações (Customer/Payment/Subscription/Notification)` | Trocar `PostAsync` por `PutAsync` em 6 lugares + ajustar testes para `AssertRequestMethod("PUT")` | Updates passam a funcionar |
| 2 | `fix(fiscalInfo): rota correta /fiscalInfo` | Corrigir constante de rota + mover `ListMunicipalServices` de Invoice para FiscalInfo | FiscalInfo passa a funcionar |
| 3 | `fix(finance): Balance retorna objeto { balance }` | Criar tipo `Balance` + atualizar manager | `Balance()` retorna valor correto |
| 4 | `fix(anticipation): remover SignAgreement (endpoint inexistente)` | Remover método (ou confirmar com Asaas) | Sem 404 silencioso |
| 5 | `feat(installments): adicionar Create + endpoints faltantes` | Adicionar `Create()`, `CreateWithCreditCard()`, `CancelPayments()`, `ListPayments()`, `UpdateSplits()` | Manager passa a ser usável |

### Sprint 2 — Reescrita do WebhookManager

| # | PR | O quê |
|---|---|---|
| 6 | `refactor!(webhooks): migrar para CRUD por ID /v3/webhooks` | Reescrever manager + modelos + enums (`WebhookEvent` com ~100 valores, `WebhookSendType`). **Breaking change** — anunciar major version. Manter os métodos antigos `[Obsolete]` por uma minor antes de remover. |

### Sprint 3 — Endpoints faltantes em managers existentes

| # | PR | O quê |
|---|---|---|
| 7 | `feat(payment): documentos, simulate, limits, billingInfo, viewingInfo, status, refunds, bankSlip/refund` | 13 endpoints novos |
| 8 | `feat(subscription): updateCreditCard` | 1 endpoint |
| 9 | `feat(anticipation): cancel, limits, configurations` | 4 endpoints |
| 10 | `feat(creditCard): preAuthorization config` | 2 endpoints |
| 11 | `feat(transfer): cancel + rota /transfers/ separada` | 1 endpoint + correção de roteamento |
| 12 | `feat(pix): qrCode delete, transaction find, tokenBucket` | 3 endpoints |
| 13 | `feat(myAccount): commercialInfo, status, documents` | 7 endpoints + correção de rota raiz |
| 14 | `feat(asaasAccount): find by id, accessTokens, resendActivationLink` | 5 endpoints |

### Sprint 4 — Novos domínios

| # | PR | O quê |
|---|---|---|
| 15 | `feat: ChargebackManager` | 3 endpoints |
| 16 | `feat: EscrowManager` | 5 endpoints |
| 17 | `feat: CheckoutManager` | 2 endpoints |
| 18 | `feat: MobilePhoneRechargeManager` | 4 endpoints |
| 19 | `feat: SandboxManager (helpers para testes)` | 3 endpoints |
| 20 | `feat(payment): splits queries` | 4 endpoints (adicionar em `PaymentManager`) |

### Sprint 5 — Pix evolução recente

| # | PR | O quê |
|---|---|---|
| 21 | `feat(pix): PixAutomaticManager` | 6 endpoints (autorizações + payment instructions) |
| 22 | `feat(pix): PixRecurringManager` | 5 endpoints (recurrings + items) |

### Sprint 6 — Polimento e quality

| # | PR | O quê |
|---|---|---|
| 23 | `refactor(models): completar campos faltantes em Customer/Payment/Notification/Webhook` | Completar response DTOs |
| 24 | `refactor(models): bool → bool? em campos opcionais; revisar DateTime` | Reduz NREs e bugs de parsing |
| 25 | `refactor(core): IHttpClientFactory` | Anti-pattern de `new HttpClient()` |
| 26 | `chore: corrigir WasSucessfull → WasSuccessful (manter alias)` | Polimento sem breaking |

---

## 7. Verificação após cada Sprint

```powershell
dotnet build Codout.Apis.Asaas/
dotnet test Codout.Apis.Asaas.Tests/
```

Para cada PR de bug bloqueante (Sprint 1): adicionar/ajustar teste em `ManagerTestBase<T>` que `AssertRequestMethod` e `AssertRequestUrl` validem método HTTP e rota. Hoje muitos testes assumem "qualquer POST" funciona.

Idealmente, criar um teste de "smoke" que confronte cada método público de manager com a lista de endpoints documentada (poderia gerar essa lista a partir do MCP do Asaas em CI).

---

## 8. Anexo — Cobertura por Manager

| Manager | Endpoints SDK | Endpoints API | Cobertura | Status |
|---|---|---|---|---|
| CustomerManager | 6 | 7 | 86% | 🔴 1 bug PUT + 1 endpoint faltando |
| PaymentManager | 11 | 24 | 46% | 🔴 1 bug PUT + 13 endpoints faltando |
| SubscriptionManager | 10 | 11 | 91% | 🔴 2 bugs PUT |
| InstallmentManager | 5 | 8 | 62% | 🔴 Sem Create |
| PixManager | 10 | 22 | 45% | 🟡 Faltam Automatic + Recurring |
| WebhookManager | 6 | 6 | 0% efetivo | 🔴 Tudo em rota errada |
| TransferManager | 4 | 5 | 80% | 🟡 falta cancel + roteamento /transfers/ |
| WalletManager | 1 | 1 | 100% | ✅ |
| AnticipationManager | 5 | 6 | 50% | 🔴 endpoint inexistente + 4 faltando |
| BillPaymentManager | 5 | 5 | 100% | ✅ |
| CreditCardManager | 1 | 3 | 33% | 🟡 Pré-autorização config faltando |
| PaymentLinkManager | 9 | 9 | 100% | ✅ |
| NotificationManager | 2 | 2 | 0% efetivo | 🔴 2 bugs PUT |
| InvoiceManager | 7 | 6 | — | 🔴 1 endpoint em rota errada |
| PaymentDunningManager | 8 | 8 | 100% | ✅ |
| FinanceManager | 4 | 4 | 100% | 🔴 1 bug response shape |
| MyAccountManager | 4 | 11 | 36% | 🔴 Find em rota errada + 7 endpoints faltando |
| AsaasAccountManager | 2 | 6 | 33% | 🔴 Faltam Find + accessTokens + resend |
| CreditBureauReportManager | 3 | 3 | 100% | ✅ |
| CustomerFiscalInfoManager | 3 | 9 | 0% efetivo | 🔴 Rota errada + 6 endpoints faltando |

**Domínios completamente ausentes:** Chargebacks, Escrow, Checkouts, MobilePhoneRecharges, Splits queries, Lean Payments, Sandbox helpers.
