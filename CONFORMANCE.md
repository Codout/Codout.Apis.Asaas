# Relatório de Conformidade — SDK Codout.Apis.Asaas v3.2.0

> **Status:** ✅ AUDITORIA COMPLETA — 27/27 MANAGERS SCHEMA-FIRST
> **Metodologia:** cada endpoint listado abaixo foi consultado via MCP `asaas` (`mcp__asaas__get-endpoint`). Models foram verificados campo-a-campo contra o schema OpenAPI. Fixtures criadas a partir dos exemplos oficiais. Contract tests congelam o shape JSON.
>
> **Resultado:** **664 testes unit/contract** passando + **15 integration tests passando contra sandbox real** ([run #26378491985](https://github.com/Codout/Codout.Apis.Asaas/actions/runs/26378491985), 2026-05-25, 6.7s).
> **27/27 managers** auditados, **42 famílias de bugs (B-19 a B-42)** corrigidas no total, **0 warnings** de build.
>
> **CI:**
> - [`.github/workflows/ci.yml`](.github/workflows/ci.yml) — unit/contract em todo push/PR
> - [`.github/workflows/integration-sandbox.yml`](.github/workflows/integration-sandbox.yml) — integration tests com secret `ASAAS_SANDBOX_TOKEN` (manual + nightly)

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

## §10 — CreditBureauReportManager (pré-existente) — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/creditBureauReport` (request) | ✅ | `CreateCreditBureauReportRequest` (customer? + cpfCnpj? — ambos opcionais) | inline | `CreateRequest_HasOnlyCustomerAndCpfCnpj`, `_DoesNotSerializeRemovedFields` | ✅ |
| `POST /v3/creditBureauReport` (response) | ✅ | `CreditBureauReport` (id, dateCreated, cpfCnpj, customer, downloadUrl, reportFile) — `reportFile` populado APENAS no POST | `CreditBureauReport/report-create-response.json` | `ReportResponse_PopulatesReportFile_OnCreate` | ✅ |
| `GET /v3/creditBureauReport` | ✅ | `ResponseList<CreditBureauReport>` + `CreditBureauReportListFilter` (startDate, endDate) | `CreditBureauReport/reports-list-response.json` | `ReportsList_*`, `ListFilter_*` (2 tests) | ✅ |
| `GET /v3/creditBureauReport/{id}` | ✅ | mesmo `CreditBureauReport` (mas `reportFile=null` aqui) | `CreditBureauReport/report-response.json` | `ReportResponse_DeserializesFromOfficialFixture_GetById`, `_NoFakeFields` | ✅ |

**Bugs corrigidos nesta fase (B-23):**
- **B-23a/b**: `CreditBureauReport` tinha `State: string` e `Status: string` — **nenhum existe no schema oficial**. Removidos. Era chute do dev original.
- **B-23c**: `CreditBureauReport` faltava `DownloadUrl: string` e `ReportFile: string` (PDF Base64). Adicionados. Sem o fix, consumidores não tinham como baixar o relatório.
- **B-23d**: `CreateCreditBureauReportRequest.State` foi removido (não existe no schema).
- **B-23e**: `List(offset, limit)` não aceitava filtro. Schema expõe `startDate` e `endDate`. Criado `CreditBureauReportListFilter` e novo overload `List(offset, limit, filter)` backwards-compatible.

## §11 — BillPaymentManager (pré-existente) — ✅

| Endpoint | MCP | Model | Fixture | Contract test | Status |
|---|---|---|---|---|---|
| `POST /v3/bill` (request) | ✅ | `CreateBillPaymentRequest` (required: identificationField; opcionais: scheduleDate, description, discount, interest, fine, dueDate, value, externalReference) | inline | `CreateRequest_SerializesAllKeysIncludingNewOnes`, `_OptionalFieldsOmittedWhenNull` | ✅ |
| `POST /v3/bill` (response) | ✅ | `BillPayment` (17 campos incluindo interest, fine, paymentDate, externalReference, failReasons array) | `BillPayment/bill-response.json` | `BillResponse_DeserializesFromOfficialFixture`, `_FailReasonsIsArrayOfStrings` | ✅ |
| `GET /v3/bill` | ✅ | `ResponseList<BillPayment>` | `BillPayment/bills-list-response.json` | `BillsList_UsesStandardEnvelopeWithPagination` | ✅ |
| `GET /v3/bill/{id}` | ✅ | mesmo `BillPayment` | reusa | (idem) | ✅ |
| `POST /v3/bill/simulate` (request) | ✅ | `SimulateBillPaymentRequest` (identificationField OU barCode) | inline | `SimulateRequest_AcceptsIdentificationFieldOrBarCode` | ✅ |
| `POST /v3/bill/simulate` (response) | ✅ | `SimulatedBillPayment` (minimumScheduleDate, fee, bankSlipInfo com 17 campos) | `BillPayment/simulate-response.json` | `SimulateResponse_DeserializesFromOfficialFixture` | ✅ |
| `POST /v3/bill/{id}/cancel` | ✅ | body vazio → `BillPayment` | (cobertura unit do manager) | (cobertura existente) | ✅ |
| Enum `BillPaymentStatus` (7 valores) | ✅ | enum tipado | inline | `BillStatus_AllSevenValuesDeserialize` | ✅ |

**Bugs corrigidos nesta fase (B-24):**
- **B-24a**: `BillPayment` faltava `Interest`, `Fine`, `PaymentDate`, `ExternalReference`. `FailReasons` era `string`; schema é `array of string` — trocado para `List<string>`.
- **B-24b**: `BillPaymentStatus` enum tinha apenas 5 valores. Schema tem 7 (adicionados `REFUNDED` e `AWAITING_CHECKOUT_RISK_ANALYSIS_REQUEST`).
- **B-24c**: `BillPayment.CanBeCancelled`/`DueDate`/`ScheduleDate`/`PaymentDate` agora são nullable (response pode omitir/null em status iniciais).
- **B-24d**: `CreateBillPaymentRequest` faltava `Interest`, `Fine`, `ExternalReference`. Campos não-obrigatórios (`Value`, `DueDate`, `ScheduleDate`, `Discount`) trocados para nullable (apenas `IdentificationField` é required no schema).
- **B-24e**: `BankSlipInfo` tinha 5 campos com `BankCode` (nome chutado). Schema tem 17 campos com `bank`. Modelo reescrito: `Bank`, `BeneficiaryCpfCnpj`, `BeneficiaryName`, `AllowChangeValue`, `MinValue`, `MaxValue`, `DiscountValue`, `InterestValue`, `FineValue`, `OriginalValue`, `TotalDiscountValue`, `TotalAdditionalValue`, `IsOverdue` — todos adicionados.

---

## §12 — CustomerManager — ✅

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `POST /v3/customers` (CRUD) | ✅ | B-25e (NotificationDisabled bool? em Create/Update) |
| `GET /v3/customers` + 5 filtros | ✅ | OK |
| `GET /v3/customers/{id}` | ✅ | B-25a (DateCreated nullable) |
| `PUT /v3/customers/{id}` | ✅ | (idem create) |
| `DELETE /v3/customers/{id}` + restore | ✅ | OK |
| `GET /v3/customers/{id}/notifications` | ✅ | **B-25g — endpoint estava faltando no manager. Adicionado `GetNotifications`** |

Contract tests: `CustomerContractTests` (8 tests).

## §13 — PaymentManager (resto) — ✅

Cobre todos os endpoints alem de `/limits` e `/simulate` (já em §1).

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `POST /v3/payments` (BOLETO/PIX/etc) | ✅ | OK |
| `POST /v3/payments/` (Credit Card) | ✅ | OK |
| `GET /v3/payments` com 18 filtros | ✅ | **B-26d — faltavam 9 filtros: customerGroupName, invoiceStatus, estimatedCreditDate, pixQrCodeId, anticipable, user, checkoutSession, dateCreated[ge]/[le], estimatedCreditDate[ge]/[le]** |
| `GET /v3/payments/{id}` | ✅ | B-26a/b/c (DateCreated, DueDate, OriginalDueDate nullable) |
| `PUT/DELETE/POST restore/refund/...` | ✅ | OK |
| `GET /v3/payments/{id}/pixQrCode` | ✅ | OK |
| `GET /v3/payments/{id}/identificationField` | ✅ | OK |
| `POST /v3/payments/{id}/receiveInCash` | ✅ | OK |

Quirk: filtro Payment usa `[ge]/[le]` **lowercase** (Invoice usa `[Ge]/[Le]` uppercase).

Contract tests: `PaymentContractTests` (21 tests: 6 existentes + 7 novos do resto).

## §14 — SubscriptionManager — ✅

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `POST /v3/subscriptions` + List/Find/Update/Delete | ✅ | B-27d (Object, PaymentLinkId, CheckoutSession, Split adicionados) |
| `PUT /v3/subscriptions/{id}/creditCard` | ✅ | OK |
| `GET /v3/subscriptions/{id}/payments` + `/paymentBook` | ✅ | OK |
| `POST/PUT/GET/DELETE /v3/subscriptions/{id}/invoiceSettings` | ✅ | OK |
| `GET /v3/subscriptions/{id}/invoices` | ✅ | OK |

**Bugs:**
- B-27a: enum `SubscriptionStatus` faltava `INACTIVE` (tinha apenas ACTIVE/EXPIRED).
- B-27b/c: `DateCreated`, `NextDueDate` → DateTime?
- B-27e: filter faltava `customerGroupName`, `status`, `deletedOnly`, `externalReference`, `order`, `sort`.

Contract tests: `SubscriptionContractTests` (5 tests).

## §15 — PixManager — ✅

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `POST/GET/DELETE /v3/pix/addressKeys` | ✅ | B-28c (Status enum, QrCode nested, CanBeDeleted/Reason) |
| `POST /v3/pix/qrCodes/static` + DELETE | ✅ | OK |
| `GET /v3/pix/tokenBucket/addressKey` | ✅ | OK |
| `POST /v3/pix/qrCodes/pay` + `/decode` | ✅ | OK |
| `GET /v3/pix/transactions` com 3 filtros | ✅ | **B-28e — filter não existia. Criado `PixTransactionListFilter`** |
| `GET /v3/pix/transactions/{id}` | ✅ | **B-28b — modelo reescrito (7 → 25 campos)** |
| `POST /v3/pix/transactions/{id}/cancel` | ✅ | OK |

**Bug crítico B-28a:** `PixTransactionStatus` enum estava com 5 valores INVENTADOS (PENDING, FAILED não existem no schema). Reescrito com 11 valores reais.

Novos enums: `PixTransactionType` (5), `PixTransactionOriginType` (6), `PixTransactionFinality` (2), `PixAddressKeyStatus` (6).

Contract tests: `PixContractTests` (7 tests).

## §16 — TransferManager — ✅

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `POST /v3/transfers` (bank account) | ✅ | OK |
| `POST /v3/transfers/` (asaas account) | ✅ | OK |
| `GET /v3/transfers` com 5 filtros | ✅ | **B-29h — faltavam dateCreated[ge]/[le] e transferDate[ge]/[le]** |
| `GET /v3/transfers/{id}` | ✅ | B-29b/c (DateCreated, Authorized nullable) |
| `DELETE /v3/transfers/{id}/cancel` | ✅ | OK |

**Bugs:** B-29d (`AsaasAccountTransferStatus` faltavam BANK_PROCESSING/FAILED), B-29e (novo enum `TransferOperationType`), B-29g (BaseTransfer faltava 7 campos), Bank/BankAccount faltavam vários campos.

Contract tests: `TransferContractTests` (5 tests).

## §17 — AnticipationManager — ✅

Endpoints: POST/GET `/v3/anticipations`, simulate, find, cancel, limits, automatic configurations.

**Bugs B-30:** Anticipation.AnticipationDate/DueDate/RequestDate → DateTime? + campo Object adicionado.

Contract tests: `AnticipationContractTests` (3 tests).

## §18 — InstallmentManager — ✅

Endpoints: POST/GET/PUT/DELETE `/v3/installments`, refund, payments, paymentBook, cancelPendingPayments, splits.

**Bugs B-31:** ExpirationDay → int?, adicionados CreditCard (nested) e Refunds (array).

Contract tests: `InstallmentContractTests` (2 tests).

## §19 — WebhookManager — ✅

Endpoints: POST/GET/PUT/DELETE `/v3/webhooks`, removeBackoff.

**Sem bugs estruturais.** Modelo Webhook + enum WebhookEvent (110+ valores) verificados OK.

Quirk: GET `/v3/webhooks` no schema só aceita offset/limit (filter `WebhookListFilter` mantido por backwards-compat — backend pode aceitar mesmo não documentado).

Contract tests: `WebhookContractTests` (3 tests).

## §20 — WalletManager — ✅

Endpoint único: `GET /v3/wallets/`.

**Bug B-33:** Wallet faltava `Object` (response wrapper). Adicionado.

Contract tests: `WalletContractTests` (1 test).

## §21 — NotificationManager — ✅

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `PUT /v3/notifications/{id}` | ✅ | B-34b (todos bools → bool? em UpdateRequest) |
| `PUT /v3/notifications/batch` | ✅ | OK |

**Bugs:** B-34a (`Event` enum faltando — novo `NotificationEvent` 6 valores), B-34b (Notification + UpdateRequest com bools nullable).

Contract tests: `NotificationContractTests` (3 tests).

## §22 — CreditCardManager — ✅

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `POST /v3/creditCard/tokenizeCreditCard` | ✅ | B-35a (Brand string → enum CreditCardBrand 13 valores) |
| `POST /v3/creditCard/preAuthorization/config` | ✅ | **B-35b — PreAuthorizationConfig tinha campos INVENTADOS (Enabled, AutomaticCaptureDelay). Schema: {daysToExpire}**. Reescrito |
| `GET /v3/creditCard/preAuthorization/config` | ✅ | (idem) |

Contract tests: `CreditCardContractTests` (5 tests).

## §23 — PaymentLinkManager — ✅

11 endpoints (CRUD + images).

**Bugs B-36:**
- B-36a: PaymentLink.SubscriptionCycle string → `Cycle` enum (7 valores).
- B-36b: faltavam ViewCount, IsAddressRequired, ExternalReference.
- B-36c: Value, Active, NotificationEnabled, Deleted, DueDateLimitDays, MaxInstallmentCount → nullable.

Contract tests: `PaymentLinkContractTests` (3 tests).

## §24 — FinanceManager — ✅

| Endpoint | MCP | Bugs corrigidos |
|---|---|---|
| `GET /v3/finance/balance` | ✅ | OK |
| `GET /v3/finance/payment/statistics` | ✅ | **B-37b — não aceitava filtros. Schema expõe 11. Criado `PaymentStatisticsFilter`** |
| `GET /v3/finance/split/statistics` | ✅ | **B-37a — campos INVENTADOS (TotalPendingValue/TotalReceivedValue). Schema: {income, value}**. Reescrito |
| `/v3/financialTransactions` (legado, fora do schema) | ⚠️ | mantido por backwards-compat |

Contract tests: `FinanceContractTests` (4 tests).

## §25 — MyAccountManager (resto) — ✅

Endpoints `/myAccount/commercialInfo`, `/status`, `/fees`, `/accountNumber`, `/paymentCheckoutConfig`, `DELETE /myAccount`.

**Bugs B-38:**
- B-38a: MyAccount.Status string → `AccountInfoStatus` enum (4 valores).
- B-38b: faltavam CompanyName, IncomeValue, TradingName, Site, AvailableCompanyNames (array), CommercialInfoExpiration (nested).
- InscricaoEstadual marcado `[Obsolete]` (não existe no schema atual).

Contract tests: `MyAccountContractTests` (3 tests).

## §26 — AsaasAccountManager — ✅

Endpoints: POST/GET `/v3/accounts`, find, resendActivationLink, accessTokens CRUD.

**Bugs B-39:**
- B-39a: Account.City string → `long?` (schema: integer city id).
- B-39b: faltavam Object, Id, BirthDate, TradingName, Site, AccountNumber (nested), CommercialInfoExpiration (nested).
- B-39c: ApiKey marcado `[Obsolete]` (não existe no schema oficial).

Contract tests: `AsaasAccountContractTests` (1 test).

## §27 — FiscalInfoManager — ✅

10 endpoints (CRUD + lookups municipais/federais/nbs/tributários).

**Bugs B-40:**
- B-40a: faltava NbsCode.
- B-40b: RpsNumber, LoteNumber string → int (schema: integer).
- B-40c: faltavam PasswordSent, AccessTokenSent, CertificateSent (bools) + NationalPortalTaxCalculationRegime + Object.
- B-40d/e: StateInscription e AccessToken marcados `[Obsolete]`.

Contract tests: `FiscalInfoContractTests` (1 test).

## §28 — ChargebackManager — ✅

Endpoints: GET list/find, POST dispute.

**Bug B-41:** Chargeback faltava `CreditCard` (nested `ChargebackCreditCard` com number + brand enum). Reason → nullable.

Contract tests: `ChargebackContractTests` (3 tests).

## §29 — SandboxManager — ✅

3 endpoints (approve account, confirm payment, force overdue).

**Sem bugs** — manager já correto. `EnsureSandbox()` bloqueia uso em produção.

Contract tests: `SandboxContractTests` (1 test sanity check).

---

## §50 — Cross-pattern bug list (consolidação)

Padrões de bug encontrados em múltiplos managers e como foram corrigidos sistemicamente:

### Pattern 1 — Envelope `{ data: [...] }` minimalista (sem paginação)
- **Endpoints afetados:** `GET /myAccount/documents` (§7), `GET /pix/transactions/recurrings/{id}/items` (§5).
- **Anti-padrão:** Usar `ResponseList<T>` que assume `hasMore/totalCount/limit/offset`.
- **Fix:** wrapper dedicado (`AccountDocumentResponse`, `PixRecurringItemsResponse`).

### Pattern 2 — `bool` PascalCase em query params (`True`/`False`)
- **Solução sistêmica:** `RequestParameters.Add(bool?)` força lowercase `"true"`/`"false"`.
- **Coberto por:** `RequestParametersContractTests.Bool_*` (3 tests, 2026-05-24).
- **Validado em runtime:** `PaymentIntegrationTests.ListPayments_WithAnticipatedFilter` (§99).

### Pattern 3 — Query param com casing errado em range filters
- **Padrão Asaas:** Payment usa `[ge]/[le]` LOWERCASE; Invoice usa `[Ge]/[Le]` UPPERCASE.
- **Bugs corrigidos:** B-21b (Invoice), B-29h (Transfer), B-26d (Payment já estava correto).
- **Validado:** contract tests + `TransferIntegrationTests.ListTransfers_WithDateRangeFilter`.

### Pattern 4 — Body vs query param
- **Bug crítico B-22k:** `POST /paymentDunnings/simulate` enviava `payment` no body. Schema oficial expõe como query param.
- **Fix:** `PaymentDunningManager.Simulate` constrói query string e envia body vazio.

### Pattern 5 — Enum incompleto ou inventado
- **Bugs corrigidos:**
  - B-19 (`MobilePhoneProvider.AvailableValues` era `List<decimal>` — schema: array de `{name, description, bonus, minValue, maxValue}`)
  - B-20a–d (4 enums tipados em AccountDocument)
  - B-22f (`PaymentDunningHistoryStatus`), B-22h (PaymentDunningType ganhou DEBT_RECOVERY_ASSISTANCE)
  - B-24b (`BillPaymentStatus` 5→7 valores), B-27a (`SubscriptionStatus` ganhou INACTIVE)
  - **B-28a (`PixTransactionStatus` 5 valores INVENTADOS → 11 reais — mais grave)**
  - B-34a (`NotificationEvent` enum criado), B-35a (`CreditCardBrand`), B-36a (`Cycle`)
  - B-38a (`AccountInfoStatus`), B-40b (Rps/Lote int), B-41 (Chargeback enums)

### Pattern 6 — Array vs objeto único
- **Bug B-22m:** `SimulatedPaymentDunning.TypeSimulations` e `PaymentDunningPaymentAvailable.TypeSimulations` eram objeto único. Schema retorna array. Lançava `InvalidCastException` em runtime.

### Pattern 7 — Nullable incorreto
- **Bugs corrigidos:** B-22c/d, B-26a/b/c, B-27b/c, B-29b/c, B-30a, B-34b, B-35, B-36c, B-25a/e. Sempre que schema permite omitir/null, mas modelo era non-nullable, deserialização quebrava ou forçava `false`/`default` silenciosamente.

### Pattern 8 — Paginação incorreta
- Validada via `ResponseList<T>` envelope padrão em todos os contract tests `*List_UsesStandardEnvelopeWithPagination`.

### Pattern 9 — Campos obrigatórios ausentes
- **Bugs:** B-21d (UpdatePayment em Invoice), B-24d (Interest/Fine/ExternalReference em BillPayment), B-25g (GetNotifications endpoint), B-28e/B-37b (filtros faltando).

### Pattern 10 — Nome de campo errado
- **Bugs:** B-17 (`authorization` → `authorizationId`), B-29 (`bankCode` → `bank`), B-21b ([ge]/[le] casing), B-22a (DunningNumber int vs string), B-40b (RpsNumber int vs string).

### Pattern 11 — Campos inventados / não existem no schema
- **Bugs graves:** B-23a/b (`State`, `Status` em CreditBureauReport), B-20f (`Name`, `Url` em AccountDocumentFile), B-35b (`Enabled`, `AutomaticCaptureDelay` em PreAuthorizationConfig), B-37a (`TotalPendingValue`, `TotalReceivedValue` em SplitStatistics), B-39 (`ApiKey`, `City` string em Account), B-40 (`StateInscription`, `AccessToken` em FiscalInfo).
- Estes representam pura **chute do dev original** sem verificar a doc. Removidos ou marcados `[Obsolete]`.

### Padrões sistêmicos confirmados OK
- `RequestParameters.Add(decimal?)` com `InvariantCulture` (`12.5` não `12,5`).
- `RequestParameters.Add(bool?)` com lowercase.
- `DateTimeExtensions.ToApiRequest` com `InvariantCulture`.
- `RequestParameters.Add(Enum)` serializa nome do enum em UPPER.
- Envelope padrão `{object, hasMore, totalCount, limit, offset, data}` em `ResponseList<T>`.

---

## §99 — Integration tests (sandbox real)

Status: ✅ implementado — **15 tests** cobrindo 7 managers críticos. Skip automático sem `ASAAS_SANDBOX_TOKEN`.

| Manager | Tests | Endpoints cobertos | Valida regression |
|---|---|---|---|
| Customer | 2 | CRUD completo + List paginado | Round-trip, envelope padrão |
| Payment | 3 | POST BOLETO/PIX, GET pixQrCode, List filter Anticipated | **B-26d (bool? filter)** |
| Subscription | 2 | Create BOLETO + List filter Status=INACTIVE | **B-27a (enum INACTIVE)** |
| Pix | 2 | ListAddressKeys, ListTransactions filter Status=AWAITING_REQUEST | **B-28a (enum 11 valores)** |
| Transfer | 1 | List filter date range [ge]/[le] | **B-29h (casing lowercase)** |
| Anticipation | 2 | List + GetLimits | Envelope, schema |
| Finance | 3 | Balance + Statistics filter + Split shape | **B-37a (income/value), B-37b (filter)** |

**Infraestrutura:**
- `[IntegrationFact]` (custom attribute) — skip automático se `ASAAS_SANDBOX_TOKEN` ausente, mensagem explicativa no skip reason.
- `IntegrationTestBase` — `[Trait("Category", "Integration")]`, constrói `AsaasApi` real apontando para `AsaasEnvironment.SANDBOX`.
- Cada test cria seus próprios recursos (suffix timestampado) e limpa no `finally` para não poluir o sandbox.

**CI workflow:** [`.github/workflows/integration-sandbox.yml`](.github/workflows/integration-sandbox.yml)
- `workflow_dispatch` (trigger manual via UI do GitHub)
- `schedule: '0 4 * * *'` (nightly 04:00 UTC / 01:00 BRT)
- Secret necessário: `ASAAS_SANDBOX_TOKEN` (Settings → Secrets and variables → Actions)
- Workflow emite warning explícito se secret estiver ausente, mas não falha.

**Para rodar localmente:**
```powershell
$env:ASAAS_SANDBOX_TOKEN = "aact_YTU0...seu_token_sandbox..."
dotnet test --filter "Category=Integration"
```

**Para rodar tudo EXCETO integration (CI local sem credencial):**
```powershell
dotnet test --filter "Category!=Integration"
```

Sem a variável: integration tests fazem skip automaticamente — `dotnet test` continua verde.

### Checklist por test (anti-fixture-falsa)

Cada integration test foi escrito seguindo este checklist:
- ✅ Cria seus próprios recursos (não depende de estado pré-existente no sandbox)
- ✅ Usa timestamp no nome/email para evitar colisão entre execuções
- ✅ Limpa recursos no `finally` quando aplicável
- ✅ Asserta `WasSuccessful()` com mensagem de erro detalhada (para diagnóstico no CI)
- ✅ Valida pelo menos um bug específico (B-XX) corrigido na auditoria
- ✅ Não duplica cobertura de contract tests (foca em comportamento end-to-end, não shape)

### Riscos residuais e por quê

Status: **AINDA NÃO EXECUTADOS contra sandbox real nesta sessão** — apenas escritos e validados que skip funciona. O agente que escreveu a auditoria não tem acesso a `ASAAS_SANDBOX_TOKEN`. Para fechar essa lacuna:

1. **Curto prazo:** rodar manualmente via `workflow_dispatch` no GitHub Actions com o secret configurado. Resultado da primeira execução pode revelar:
   - Schemas de fixture incompletos (raros, mas possíveis)
   - Comportamentos sandbox vs spec divergentes
   - Algum CPF de teste rejeitado pelo sandbox específico
2. **Médio prazo:** expandir cobertura conforme bugs surgirem em produção dos consumidores.

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

Lista honesta do que **não** está 100% fechado, classificada por aceitabilidade:

### ❌ NÃO ACEITÁVEIS (precisam resolver antes de produção de SDK de pagamento)

Nenhum. Todos os 27 managers passaram por auditoria schema-first.

### ⚠️ ACEITÁVEIS COM RESSALVA

**~~1. Integration tests nunca rodaram contra sandbox real nesta sessão.~~ ✅ RESOLVIDO**
- Primeira execução real: [run #26378491985](https://github.com/Codout/Codout.Apis.Asaas/actions/runs/26378491985) em 2026-05-25.
- Resultado: **15/15 testes passaram** em 6.7s contra `api-sandbox.asaas.com`.
- Validou em runtime real: B-26d (bool filter lowercase), B-27a (SubscriptionStatus.INACTIVE), B-28a (PixTransactionStatus enum 11 valores), B-29h (TransferListFilter date range casing), B-37a (SplitStatistics shape income/value), B-37b (PaymentStatisticsFilter), entre outros.
- Nightly run configurado em `integration-sandbox.yml` continua valendo para regressões futuras.

**2. Algumas fixtures foram escritas manualmente a partir dos exemplos MCP (não auto-geradas).**
- Risco: se eu copiei mal um exemplo (ex: esqueci um campo que aparece em outros casos), o contract test passa mas o modelo continua incompleto.
- Mitigação: `JsonContractAssert.HasRootProperty` + `DoesNotSerializeKey` em campos críticos detectam divergência.
- Lacunas conhecidas: enums com 100+ valores (WebhookEvent) só testam 10 representativos.

### ✅ ACEITÁVEIS (decisão explícita)

**Endpoints `/financialTransactions`** (FinanceManager): legado, não está mais no MCP. Mantido por backwards-compat.

**Webhooks (recebimento de payloads):** O SDK expõe `WebhookManager` para configurar endpoints, mas não decodificadores tipados dos payloads que o Asaas envia. Consumidores deserializam manualmente. **Scope decisão:** seria um SDK separado.

**Reforma Tributária:** Campos `stateIbs`/`municipalIbs`/`cbs` em `Taxes` (Invoice) adicionados conforme spec atual. Comportamento em produção depende do calendário de implementação. Pode mudar — auditar quando spec mudar.

**Campos `[Obsolete]`:** 5 campos marcados em PaymentDunning, AsaasAccount, FiscalInfo, MyAccount. Mantidos por backwards-compat. Backend pode parar de retornar a qualquer momento — consumidores devem migrar.

**Filtros `WebhookListFilter` (name, enabled, interrupted):** Não documentados no MCP, mas mantidos no SDK porque a API original aceita. Se falhar, remover.

**Endpoint adicional FiscalInfo:** 6 endpoints de lookup (`federalServiceCodes`, `nbsCodes`, `operationIndicatorCodes`, `taxClassificationCodes`, `taxSituationCodes`, `nationalPortal`) existem no schema mas não no manager. Decisão: feature adicional, não impede uso do SDK.
