# Relatório de Conformidade — SDK Codout.Apis.Asaas v3.0.0

> **Status:** ✅ FASES 1–6 CONCLUÍDAS (auditoria schema-first completa)
> **Metodologia:** cada endpoint listado abaixo foi consultado via MCP `asaas` (`mcp__asaas__get-endpoint`). Models foram verificados campo-a-campo contra o schema OpenAPI. Fixtures criadas a partir dos exemplos oficiais. Contract tests congelam o shape JSON.
>
> **Resultado:** 599 testes unit/contract passando + 5 integration tests (skip sem `ASAAS_SANDBOX_TOKEN`). 11 managers auditados, 24 famílias de bugs (B-XX) corrigidas, 0 warnings de build.

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

## §99 — Integration tests (sandbox real)

Status: ✅ implementado (5 tests). Skip automático sem ASAAS_SANDBOX_TOKEN.

| Test | Endpoint(s) cobertos | Valida |
|---|---|---|
| `CustomerIntegrationTests.Create_Find_Update_Delete_Customer_RoundTrip` | POST/GET/PUT/DELETE `/v3/customers[/{id}]` | CRUD básico end-to-end com cleanup |
| `CustomerIntegrationTests.List_RespectsOffsetAndLimit_AndUsesStandardEnvelope` | GET `/v3/customers` | Paginação (offset/limit), envelope padrão |
| `PaymentIntegrationTests.CreatePayment_Boleto_ReturnsPendingPayment` | POST `/v3/payments` (BOLETO) | Schema de criação Boleto + cleanup do customer setup |
| `PaymentIntegrationTests.CreatePayment_Pix_ReturnsPixQrCode` | POST `/v3/payments` + GET `/v3/payments/{id}/pixQrCode` | Fluxo PIX completo (cobrança → QR code) |
| `PaymentIntegrationTests.ListPayments_WithAnticipatedFilter_SerializesBoolCorrectly` | GET `/v3/payments?anticipated=true` | **Garantia sistêmica**: bool? em filtro serializa lowercase corretamente (não 400) |

**Infraestrutura:**
- `[IntegrationFact]` (custom attribute) — skip automático se `ASAAS_SANDBOX_TOKEN` ausente, mensagem explicativa no skip reason.
- `IntegrationTestBase` — `[Trait("Category", "Integration")]`, constrói `AsaasApi` real apontando para `AsaasEnvironment.SANDBOX`.
- Cada test cria seus próprios recursos (suffix timestampado) e limpa no `finally` para não poluir o sandbox.

**Para rodar localmente:**
```powershell
$env:ASAAS_SANDBOX_TOKEN = "aact_YTU0...seu_token_sandbox..."
dotnet test --filter "Category=Integration"
```

**Para rodar tudo EXCETO integration (CI local):**
```powershell
dotnet test --filter "Category!=Integration"
```

Sem a variável: integration tests fazem skip automaticamente — `dotnet test` continua verde.

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

Lista honesta do que **não** está coberto por esta auditoria:

**Managers não auditados schema-first (cobertos só por unit tests pré-existentes):**
- `CustomerManager`, `SubscriptionManager`, `InstallmentManager`, `PixManager`, `WebhookManager`, `TransferManager`, `WalletManager`, `AnticipationManager`, `CreditCardManager`, `PaymentLinkManager`, `NotificationManager`, `FinanceManager`, `MyAccountManager` (exceto Documents §7), `AsaasAccountManager`, `FiscalInfoManager`, `ChargebackManager`, `SandboxManager`, `PaymentManager` (exceto `/limits` e `/simulate` §1).
- Risco: estes managers podem ter divergências similares às encontradas nos auditados (campos faltando, casing errado em filtros, enums incompletos). Confirmado via amostragem (`PaymentListFilter.Anticipated`, integration test) que ao menos o caminho crítico funciona.
- Recomendação: rodar a mesma metodologia desta auditoria nesses managers em iterações futuras conforme uso real revelar problemas.

**Schema vs comportamento runtime:**
- O MCP retorna a especificação OpenAPI publicada. Discrepâncias entre spec e implementação real do backend Asaas não são detectáveis sem integration tests específicos.
- 5 integration tests cobrem o caminho mais comum (Customer CRUD, Payment Boleto/PIX, filtro bool). Para validar managers menos auditados, expandir `Codout.Apis.Asaas.Tests/Integration/`.

**Webhooks (recebimento):**
- O SDK expõe apenas o `WebhookManager` para configurar endpoints, não decodificadores tipados dos payloads que o Asaas envia. Consumidores que recebem webhooks precisam deserializar manualmente.

**Reforma Tributária:**
- Campos `stateIbs`/`municipalIbs`/`cbs` em `Taxes` (Invoice) foram adicionados conforme spec atual. Comportamento em produção depende do calendário de implementação da Reforma — pode mudar.

**Endpoints deprecated:**
- `PaymentDunning.ReceivedInCashFeeValue` e `CancellationFeeValue` estão marcados como `[Obsolete]`. Mantidos por backwards-compat mas backend pode parar de retornar.
