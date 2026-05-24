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

## §2 — CheckoutManager — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/checkouts` (request) | ✅ | `CreateCheckoutRequest` (billingTypes, chargeTypes, callback, items required) | `Checkout/create-request-minimal.json`, `create-request-recurrent.json` | `CreateCheckoutRequest_*` (3 tests) | ✅ |
| `POST /v3/checkouts` (response) | ✅ | `Checkout` (id, link, status enum, subscriptions, customerData com city/addressNumber `int`) | `Checkout/response.json` | `CheckoutResponse_DeserializesFromOfficialFixture_FullShape`, `_UsesSplitSingular_OnResponse` | ✅ |
| `POST /v3/checkouts/{id}/cancel` | ✅ | mesmo response `Checkout`, body vazio | reusa `response.json` | (idem) | ✅ |
| Enum `CheckoutStatus` ACTIVE/CANCELED/EXPIRED/PAID | ✅ | enum tipado | inline | `CheckoutStatus_AllValuesDeserialize` | ✅ |

**Quirk documentado:** request usa `splits` (plural), response usa `split` (singular). Comentário no `CreateCheckoutRequest.cs` evita "correções" futuras erradas.

**Bug pré-existente corrigido em paralelo:** `Subscription.Enums.Cycle` estava faltando o valor `BIMONTHLY` (presente no schema oficial de `SubscriptionSaveRequestCycle` e `CheckoutSessionSubscriptionCycle`).

## §3 — EscrowManager — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/accounts/{id}/escrow` | ✅ | `SaveEscrowConfigRequest` / response `EscrowConfig` (daysToExpire required, enabled/isFeePayer optional) | `Escrow/config-request.json`, `config-response.json` | `SaveEscrowConfigRequest_HasCorrectFieldNames`, `_NoFakeFields` | ✅ |
| `GET /v3/accounts/{id}/escrow` | ✅ | `EscrowConfig` (mesmo schema) | `Escrow/config-response.json` | `EscrowConfig_DeserializesFromOfficialFixture`, `_OptionalBoolsAreNullableInResponse` | ✅ |
| `POST /v3/accounts/escrow` | ✅ | mesmo `AccountPaymentEscrowConfigDTO` | reusa | (idem) | ✅ |
| `GET /v3/accounts/escrow` | ✅ | mesmo `AccountPaymentEscrowConfigDTO` | reusa | (idem) | ✅ |
| `POST /v3/escrow/{id}/finish` | ✅ | body `{}` vazio → retorna `Payment` (não `Escrow`!) | (cobertura no unit test do manager) | (cobertura existente) | ✅ |
| `GET /v3/payments/{id}/escrow` | ✅ | `Escrow` (id, status enum, expirationDate, finishDate, finishReason enum) | `Escrow/payment-escrow-response.json` | `PaymentEscrow_*`, `EscrowStatus_*`, `EscrowFinishReason_*` (4 tests) | ✅ |
| Enum `EscrowStatus` ACTIVE/DONE | ✅ | enum tipado | inline | `EscrowStatus_BothValuesDeserialize` | ✅ |
| Enum `EscrowFinishReason` (6 valores) | ✅ | enum tipado | inline | `EscrowFinishReason_AllSixValuesDeserialize`, `_NullWhenStatusActive` | ✅ |

## §4 — PixAutomaticManager — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/pix/automatic/authorizations` | ✅ | `CreatePixAutomaticAuthorizationRequest` + `PixAutomaticAuthorization` | `PixAutomatic/authorization-create-request-minimal.json`, `authorization-response.json` | `CreateAuthorizationRequest_*` (2), `AuthorizationResponse_Deserializes*` | ✅ |
| `GET /v3/pix/automatic/authorizations` | ✅ | envelope padrão (hasMore/totalCount/limit/offset/data) | `PixAutomatic/authorizations-list-response.json` | `AuthorizationsListResponse_UsesStandardEnvelopeWithPagination` | ✅ |
| `GET /v3/pix/automatic/authorizations/{id}` | ✅ | mesmo `PixAutomaticAuthorization` | reusa | (idem) | ✅ |
| `DELETE /v3/pix/automatic/authorizations/{id}` | ✅ | retorna `PixAutomaticAuthorization` (não envelope deleted) | reusa | (idem) | ✅ |
| `GET /v3/pix/automatic/paymentInstructions/{id}` | ✅ | `PixAutomaticPaymentInstruction` (Authorization nested, dueDate, status enum, paymentId, refusalReason) | `PixAutomatic/payment-instruction-response.json` | `PaymentInstruction_DeserializesFromOfficialFixture_WithNestedAuthorization` | ✅ |
| `GET /v3/pix/automatic/paymentInstructions` (filter) | ✅ | `authorizationId`/`customerId`/`paymentId`/`status` | inline | `PaymentInstructionListFilter_SerializesAuthorizationIdNotAuthorization` | ✅ |
| Enums Status/Frequency/OriginType/PaymentInstructionStatus | ✅ | enums tipados | inline | `*_AllFiveValuesDeserialize` (3 tests) | ✅ |

**Bugs corrigidos nesta fase final:**
- **B-16**: `PixAutomaticPaymentInstruction` tinha `Authorization` como `string` + campos inventados (`Value`, `PaymentDate`, `DateCreated`, `Description`). Schema real: `Authorization` é objeto aninhado (`id`/`endToEndIdentifier`/`customerId`), `DueDate` (não `PaymentDate`), + `endToEndIdentifier`, `paymentId`, `refusalReason`. Status virou enum `PixAutomaticPaymentInstructionStatus`.
- **B-17**: `PixAutomaticPaymentInstructionListFilter` usava `authorization`/`status`. Schema real: `authorizationId`, `customerId`, `paymentId`, `status` (enum).

## §5 — PixRecurringManager — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `GET /v3/pix/transactions/recurrings` (envelope padrão) | ✅ | `ResponseList<PixRecurringTransaction>` (hasMore/totalCount/limit/offset/data) | `PixRecurring/transactions-list-response.json` | `TransactionsList_UsesStandardEnvelopeWithPagination` | ✅ |
| `GET /v3/pix/transactions/recurrings` (filtros novos) | ✅ | `PixRecurringTransactionListFilter` (status enum, value decimal invariant, searchText) | inline | `TransactionListFilter_*` (4 tests: SerializesAll, InvariantDecimal, UpperEnum, NullOmitted) | ✅ |
| `GET /v3/pix/transactions/recurrings/{id}` | ✅ | `PixRecurringTransaction` (id, status enum, origin enum, value, frequency enum, quantity, startDate, finishDate, canBeCancelled, externalAccount nested) | `PixRecurring/transaction-response.json` | `TransactionResponse_DeserializesFromOfficialFixture` | ✅ |
| `POST /v3/pix/transactions/recurrings/{id}/cancel` | ✅ | body vazio → retorna `PixRecurringTransaction` | reusa | (cobertura unit do manager) | ✅ |
| `GET /v3/pix/transactions/recurrings/{id}/items` (envelope `{data:[...]}`) | ✅ | `PixRecurringItemsResponse` wrapper (sem paginação) | `PixRecurring/items-list-envelope.json` | `ItemsListEnvelope_UsesMinimalDataOnlyShape` | ✅ |
| `POST /v3/pix/transactions/recurrings/items/{id}/cancel` | ✅ | body vazio → `PixRecurringItem` (id, status, scheduledDate, canBeCancelled, recurrenceNumber, quantity, value, refusalReasonDescription, externalAccount) | `PixRecurring/item-response.json` | `ItemResponse_DeserializesFromOfficialFixture` | ✅ |
| Enum `PixRecurringStatus` (5 valores) | ✅ | enum tipado | inline | `TransactionStatus_AllFiveValuesDeserialize` | ✅ |
| Enum `PixRecurringFrequency` (WEEKLY/MONTHLY) | ✅ | enum tipado | inline | `TransactionFrequency_BothValuesDeserialize` | ✅ |
| Enum `PixRecurringOrigin` (PIX) | ✅ | enum tipado | inline | `TransactionOrigin_PixDeserializes` | ✅ |
| Enum `PixRecurringItemStatus` (4 valores) | ✅ | enum tipado | inline | `ItemStatus_AllFourValuesDeserialize` | ✅ |

**Feature nova adicionada nesta fase:**
- **B-18**: `PixRecurringManager.List(offset, limit)` não aceitava filtro. O schema oficial expõe três filtros opcionais (`status`, `value`, `searchText`). Criado `PixRecurringTransactionListFilter` (request parameters tipado) e novo overload `List(offset, limit, filter)`. Backwards-compatible: `filter` é opcional.

**Quirks de envelope documentados:**
- `recurrings` (transactions list) usa o envelope padrão (`hasMore`/`totalCount`/`limit`/`offset`/`data`).
- `recurrings/{id}/items` usa envelope minimalista `{data:[...]}` sem paginação — comportamento diferente do schema padrão, mantido em `PixRecurringItemsResponse` wrapper para evitar deserialização incorreta via `ResponseList<T>` (regressão B-14 já fixada).

## §6 — MobilePhoneRechargeManager — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/mobilePhoneRecharges` (request) | ✅ | `CreateMobilePhoneRechargeRequest` (value+phoneNumber required) | `MobilePhoneRecharge/create-request.json` | `CreateRequest_HasRequiredFields`, `_NoFakeFields` | ✅ |
| `POST /v3/mobilePhoneRecharges` (response) | ✅ | `MobilePhoneRecharge` (id, value, phoneNumber, status enum, canBeCancelled, operatorName) | `MobilePhoneRecharge/recharge-response.json` | `RechargeResponse_DeserializesFromOfficialFixture` | ✅ |
| `GET /v3/mobilePhoneRecharges` | ✅ | `ResponseList<MobilePhoneRecharge>` envelope padrão | `MobilePhoneRecharge/recharges-list-response.json` | `RechargesList_UsesStandardEnvelopeWithPagination` | ✅ |
| `GET /v3/mobilePhoneRecharges/{id}` | ✅ | mesmo `MobilePhoneRecharge` | reusa | (idem) | ✅ |
| `POST /v3/mobilePhoneRecharges/{id}/cancel` | ✅ | body vazio → `MobilePhoneRecharge` | (cobertura unit do manager) | (cobertura existente) | ✅ |
| `GET /v3/mobilePhoneRecharges/{phoneNumber}/provider` | ✅ | `MobilePhoneProvider` (name, values: `MobilePhoneProviderValue[]` com {name, description, bonus, minValue, maxValue}) | `MobilePhoneRecharge/provider-response.json` | `ProviderResponse_DeserializesFromOfficialFixture`, `_UsesValuesNotAvailableValues` | ✅ |
| Enum `MobilePhoneRechargeStatus` (5 valores) | ✅ | enum tipado | inline | `RechargeStatus_AllFiveValuesDeserialize` | ✅ |

**Bug crítico corrigido nesta fase:**
- **B-19**: `MobilePhoneProvider` tinha `AvailableValues: List<decimal>` (chutado). Schema real: `values: array` de objetos `MobilePhoneProviderValue` com campos `{name, description, bonus, minValue, maxValue}`. Modelo completo reescrito + criada classe nova `MobilePhoneProviderValue` + test antigo do manager corrigido (não mais asserta `AvailableValues`).

## §7 — MyAccountManager (documents) — ✅

Endpoints `/v3/myAccount/documents*` (5 endpoints). Subgrupo Account Document do MyAccountManager.

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `GET /v3/myAccount/documents` | ✅ | `AccountDocumentResponse` (envelope `{rejectReasons, data:[]}` sem paginação) | `AccountDocument/pending-documents-response.json` | `PendingDocumentsResponse_DeserializesFromOfficialFixture`, `_UsesMinimalEnvelopeWithoutPagination` | ✅ |
| `POST /v3/myAccount/documents/{id}` (multipart) | ✅ | request `UploadAccountDocumentRequest` (DocumentFile + Type enum) → response `AccountDocument` | `AccountDocument/document-response.json` | `UploadRequest_HasCorrectMultipartFieldNames` | ✅ |
| `GET /v3/myAccount/documents/files/{id}` | ✅ | `AccountDocument` (id, status enum) | reusa | `DocumentResponse_HasOnlyIdAndStatus` | ✅ |
| `POST /v3/myAccount/documents/files/{id}` (multipart update) | ✅ | request `UploadAccountDocumentRequest` (DocumentFile) → response `AccountDocument` | reusa | (idem) | ✅ |
| `DELETE /v3/myAccount/documents/files/{id}` | ✅ | `BaseDeleted` ({deleted, id}) | `AccountDocument/delete-response.json` | (cobertura unit do manager) | ✅ |
| Enum `AccountDocumentStatus` (4 valores) | ✅ | enum tipado | inline | `DocumentStatus_AllFourValuesDeserialize` | ✅ |
| Enum `AccountDocumentGroupStatus` (5 valores) | ✅ | enum tipado (Group ganha IGNORED) | inline | `DocumentGroupStatus_AllFiveValuesDeserialize` | ✅ |
| Enum `AccountDocumentType` (12 valores) | ✅ | enum tipado | inline | `DocumentType_AllTwelveValuesDeserialize` | ✅ |
| Enum `AccountDocumentResponsibleType` (13 valores) | ✅ | enum tipado | inline | `ResponsibleType_AllThirteenValuesDeserialize` | ✅ |

**Bugs críticos corrigidos nesta fase (B-20):**
- **B-20a/b/c/d**: `AccountDocument.Status`, `AccountDocumentGroup.Status`, `AccountDocumentGroup.Type`, `AccountDocumentResponsible.Type` eram `string`/`List<string>`. Trocados por enums tipados.
- **B-20f**: `AccountDocumentFile` tinha campos fictícios `Name` e `Url`. Schema real retorna apenas `{id, status}`. Classe removida; endpoints passam a retornar o mesmo `AccountDocument`.
- **B-20g**: `SubmitDocument` retornava `AccountDocumentGroup`. Schema oficial retorna `AccountDocumentGetResponseDTO` (apenas `{id, status}`). Tipo de retorno do manager mudado para `AccountDocument`.
- **B-20h**: `UploadAccountDocumentRequest` tinha `DocumentType: string` e `File: IAsaasFile`. Schema espera multipart fields `type` e `documentFile`. Renomeado para `Type: AccountDocumentType?` e `DocumentFile: IAsaasFile`, alinhando os nomes após o `FirstCharToLower` do `PostMultipartFormDataContentAsync`.

**Quirk de envelope:** `/myAccount/documents` retorna envelope `{rejectReasons, data:[...]}` sem `hasMore/totalCount/limit/offset`. Mantido em `AccountDocumentResponse` para evitar deserialização incorreta via `ResponseList<T>` (B-07 já fixado em fase anterior).

## §8 — InvoiceManager (pré-existente) — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/invoices` (request) | ✅ | `CreateInvoiceRequest` (required: serviceDescription, observations, value, deductions, effectiveDate, municipalServiceName, taxes; `payment`/`customer`/`installment` opcionais) | `Invoice/schedule-request.json` | `CreateRequest_*` (3 tests) | ✅ |
| `POST /v3/invoices` (response) | ✅ | `Invoice` (id, status enum, customer, payment, taxes, etc.) | `Invoice/invoice-response.json` | `InvoiceResponse_DeserializesFromOfficialFixture`, `TaxesResponse_HasAllReformaTributariaFields` | ✅ |
| `GET /v3/invoices` (filtros) | ✅ | `InvoiceListFilter` (effectiveDate[Ge]/[Le], payment, installment, customer, externalReference, status) | inline | `ListFilter_UsesCapitalGeAndLeForEffectiveDate`, `_SupportsCustomerAndExternalReference` | ✅ |
| `GET /v3/invoices/{id}` | ✅ | mesmo `Invoice` | reusa | (idem) | ✅ |
| `PUT /v3/invoices/{id}` | ✅ | `UpdateInvoiceRequest` (todos opcionais + updatePayment) | inline | `UpdateRequest_SupportsUpdatePaymentFlag` | ✅ |
| `POST /v3/invoices/{id}/authorize` | ✅ | body vazio → `Invoice` | (cobertura unit do manager) | (cobertura existente) | ✅ |
| `POST /v3/invoices/{id}/cancel` | ✅ | body vazio → `Invoice` | (cobertura unit do manager) | (cobertura existente) | ✅ |
| Enum `InvoiceStatus` (6 valores) | ✅ | enum tipado | inline | `InvoiceStatus_AllSixValuesDeserialize` | ✅ |

**Bugs corrigidos nesta fase (B-21):**
- **B-21a**: `Taxes` model só tinha 7 campos (retainIss, iss, cofins, csll, inss, ir, pis). Schema `InvoiceTaxesResponseDTO` tem mais 6 (nbsCode, taxSituationCode, taxClassificationCode, operationIndicatorCode, pisCofinsRetentionType, pisCofinsTaxStatus) + 6 da Reforma Tributária (stateIbs, stateIbsValue, municipalIbs, municipalIbsValue, cbs, cbsValue) → todos esses sumiam silenciosamente. Modelo expandido para 19 campos.
- **B-21b**: `InvoiceListFilter` usava `effectiveDate[ge]` e `[le]` lowercase. Schema oficial usa `[Ge]` e `[Le]` maiúsculos. O filtro com casing errado era silenciosamente ignorado pela API.
- **B-21c**: `InvoiceListFilter` faltava `customer` e `externalReference`. Adicionados.
- **B-21d**: `CreateInvoiceRequest` e `UpdateInvoiceRequest` faltavam `updatePayment: bool?`. Adicionado em ambos.

## §9 — PaymentDunningManager (pré-existente) — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/paymentDunnings` (multipart) | ✅ | `CreatePaymentDunningRequest` (9 campos obrigatórios + documents binários) | (cobertura unit do manager) | `CreateRequest_UsesPaymentNotPaymentId` | ✅ |
| `POST /v3/paymentDunnings` (response) | ✅ | `PaymentDunning` (id, dunningNumber int?, status enum, type enum, payment, requestDate, value, feeValue, netValue, canBeCancelled bool?, isNecessaryResendDocumentation bool?, cannotBeCancelledReason, denialReason) | `PaymentDunning/dunning-response.json` | `DunningResponse_*` (2 tests) | ✅ |
| `GET /v3/paymentDunnings` | ✅ | `ResponseList<PaymentDunning>` + `PaymentDunningListFilter` (status, type, payment, requestStartDate, requestEndDate) | `PaymentDunning/dunnings-list-response.json` | `DunningsList_*`, `ListFilter_*` | ✅ |
| `GET /v3/paymentDunnings/{id}` | ✅ | mesmo `PaymentDunning` | reusa | (idem) | ✅ |
| `POST /v3/paymentDunnings/simulate` | ✅ | **payment como QUERY param, body vazio** → `SimulatedPaymentDunning` com `TypeSimulations: List<...>` | `PaymentDunning/simulate-response.json` | `SimulateResponse_DeserializesFromOfficialFixture` | ✅ |
| `GET /v3/paymentDunnings/{id}/history` | ✅ | `ResponseList<PaymentDunningEventHistory>` com `Status: PaymentDunningHistoryStatus` enum | `PaymentDunning/history-list-response.json` | `HistoryResponse_*`, `HistoryStatus_AllFourValuesDeserialize` | ✅ |
| `GET /v3/paymentDunnings/{id}/partialPayments` | ✅ | `ResponseList<PaymentDunningPartialPayments>` (value, description, paymentDate) | `PaymentDunning/partial-payments-response.json` | `PartialPaymentsResponse_DeserializesFromOfficialFixture` | ✅ |
| `GET /v3/paymentDunnings/paymentsAvailableForDunning` | ✅ | `ResponseList<PaymentDunningPaymentAvailable>` com `TypeSimulations: List<...>` | `PaymentDunning/payments-available-response.json` | `PaymentsAvailableResponse_DeserializesFromOfficialFixture` | ✅ |
| `POST /v3/paymentDunnings/{id}/cancel` | ✅ | body vazio → `PaymentDunning` | (cobertura unit do manager) | (cobertura existente) | ✅ |
| Enum `PaymentDunningStatus` (8 valores) | ✅ | enum tipado | inline | `DunningStatus_AllEightValuesDeserialize` | ✅ |
| Enum `PaymentDunningType` (CREDIT_BUREAU + DEBT_RECOVERY_ASSISTANCE p/ filter) | ✅ | enum tipado | inline | `DunningType_BothValuesDeserialize` | ✅ |
| Enum `PaymentDunningHistoryStatus` (4 valores) | ✅ | enum tipado | inline | `HistoryStatus_AllFourValuesDeserialize` | ✅ |

**Bugs corrigidos nesta fase (B-22):**
- **B-22a**: `PaymentDunning.DunningNumber` era `string`. Schema é `integer` (int32). Trocado para `int?`.
- **B-22b**: `PaymentDunning` faltava `CannotBeCancelledReason: string`. Adicionado.
- **B-22c/d**: `PaymentDunning.CanBeCancelled` e `IsNecessaryResendDocumentation` eram `bool` non-nullable. Schema permite null. Trocados para `bool?`. Sem o fix, omitir esses campos no JSON forçava `false` silenciosamente.
- **B-22e**: `PaymentDunning.ReceivedInCashFeeValue` e `CancellationFeeValue` marcados como `[Obsolete]` (schema oficial marca deprecated).
- **B-22f**: `PaymentDunningEventHistory.Status` era `string`. Schema é enum `PaymentDunningHistoryStatus` (IN_NEGOTIATION, NEGOTIATION_FAIL, NEGOTIATED, PAID). Trocado para enum tipado.
- **B-22h**: `PaymentDunningType` enum tinha apenas `CREDIT_BUREAU`. Filter aceita também `DEBT_RECOVERY_ASSISTANCE`. Adicionado.
- **B-22k**: `Simulate(request)` enviava `payment` no body JSON. Schema oficial expõe como **query param** (`?payment=pay_xxx`) e exige body vazio. Manager corrigido para construir query string.
- **B-22m**: `SimulatedPaymentDunning.TypeSimulations` e `PaymentDunningPaymentAvailable.TypeSimulations` eram objeto único. Schema retorna ARRAY. Trocados para `List<PaymentDunningTypeSimulations>`. Sem o fix, deserialização lançava `InvalidCastException` no JSON real.

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
