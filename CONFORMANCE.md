# Relatório de Conformidade — SDK Codout.Apis.Asaas v3.0.0

> **Status:** EM CONSTRUÇÃO (Fase 1/6 da auditoria schema-first)
> **Metodologia:** cada endpoint listado abaixo foi consultado via MCP `asaas` (`mcp__asaas__get-endpoint`). Models foram verificados campo-a-campo contra o schema OpenAPI. Fixtures criadas a partir dos exemplos oficiais. Contract tests congelam o shape JSON.

---

## Legenda

| Status | Significado |
|---|---|
| ✅ | Schema verificado via MCP, model alinhado, fixture criada, contract test passando |
| ⚠️ | Verificado mas com divergência conhecida; documentada |
| 🔴 | Divergência sem correção (não deveria existir até fim da auditoria) |
| ⏳ | Pendente nesta auditoria |

## Cobertura por nível

| Nível | O que valida | Por endpoint |
|---|---|---|
| **Contract test** | Shape JSON (chaves exatas, casing, enums, envelope) | ✅ se marcado abaixo |
| **Unit test** | Manager chama URL/método corretos, deserialização básica | ✅ pré-existente |
| **Integration test** | Chamada real contra api-sandbox.asaas.com | Apenas endpoints listados em §99 |

---

## §1 — PaymentManager (somente novos: limits, simulate) — ✅

| Endpoint | MCP consultado | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `GET /v3/payments/limits` | ✅ | `PaymentLimits` (Creation.Daily.{Limit,Used,WasReached}) | `Payment/limits-response.json` | `PaymentLimits_DeserializesFromOfficialFixture` | ✅ |
| `POST /v3/payments/simulate` (request) | ✅ | `SimulatePaymentRequest` (value, billingTypes[], installmentCount?) | `Payment/simulate-request-minimal.json`, `Payment/simulate-request-with-installments.json` | `SimulatePaymentRequest_*` (3 tests) | ✅ |
| `POST /v3/payments/simulate` (response) | ✅ | `SimulatedPayment` (value, creditCard?, bankSlip?, pix?) | `Payment/simulate-response.json` | `SimulatedPaymentResponse_DeserializesFromOfficialFixture` | ✅ |

Erros oficiais (`{errors:[{code,description}]}`) também validados via fixture compartilhada (`error-response.json`).

## §2 — CheckoutManager — ⏳

## §3 — EscrowManager — ⏳

## §4 — PixAutomaticManager — ⏳

## §5 — PixRecurringManager — ⏳

## §6 — MobilePhoneRechargeManager — ⏳

## §7 — MyAccountManager (documents) — ⏳

## §8 — InvoiceManager (pré-existente) — ⏳

## §9 — PaymentDunningManager (pré-existente) — ⏳

## §10 — CreditBureauReportManager (pré-existente) — ⏳

## §11 — BillPaymentManager (pré-existente) — ⏳

---

## §99 — Integration tests (sandbox real)

Status: ⏳ pendente Fase 4

Endpoints planejados:
- `POST /v3/customers` + `GET /v3/customers/{id}` (CRUD básico)
- `POST /v3/payments` (Boleto)
- `POST /v3/payments/` (Cartão de crédito tokenizado)
- `GET /v3/payments/{id}/pixQrCode` (PIX)
- `GET /v3/payments` com `PaymentListFilter.Anticipated = true` (bool filter)
- `GET /v3/customers` paginado (offset+limit)
- `POST /v3/sandbox/payment/{id}/confirm` (endpoint novo)

Para rodar:
```powershell
$env:ASAAS_SANDBOX_TOKEN = "aact_YTU0...seu_token_sandbox..."
dotnet test --filter "Category=Integration"
```

Sem a variável: integration tests fazem skip automaticamente.

---

## Padrões sistêmicos auditados

| Padrão | Status | Onde |
|---|---|---|
| Query params bool serializa lowercase `true`/`false` | ✅ | `RequestParameters.Add(bool?)` + `RequestParametersContractTests.Bool_*` (3 tests) |
| Query params decimal serializa invariant culture (ponto, não vírgula) | ✅ | `RequestParameters.Add(decimal?)` + `Decimal_SerializesWithDotInAllCultures` (4 culturas) |
| Query params DateTime serializa `YYYY-MM-DD` em qualquer cultura | ✅ | `DateTimeExtensions.ToApiRequest` + `DateTime_SerializesAsIsoYyyyMmDdInAllCultures` (3 culturas) |
| Query params enum serializa nome do enum em UPPER | ✅ | `RequestParameters.Add(Enum)` + `Enum_SerializesAsUppercaseAsaasName` |
| Query string escapa caracteres especiais | ✅ | `Build_BuildsCorrectQueryStringWithEscaping` |
| Envelope `{data:[...]}` (sem hasMore) — endpoints conhecidos | ✅ | `AccountDocument` (B-07 fixado), `PixRecurring.ListItems` (B-14 fixado) |
| `bool?` em campos opcionais de response | ⏳ | grep durante Fase 5 |

---

## Riscos remanescentes

A preencher ao final da auditoria.
