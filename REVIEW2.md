# Code Review 2 — branch `audit/asaas-api-conformance`

> Gerado em 2026-05-24 (após primeira revisão fechar todos os 8 🔴 + 9 🟡 + 3 🟢)
> Fontes: (1) segundo subagent code-reviewer independente, (2) verificação manual de mais schemas via MCP

---

## Sumário executivo

A primeira revisão (REVIEW.md) foi efetivamente fechada. Esta segunda passada **encontrou 6 novos schemas errados** que não tinham sido verificados antes (Escrow + PixRecurring + 1 detalhe de bool), **1 inconsistência adicional** (`Subscription.Deleted` deixado para trás), e **stale na documentação**. Tudo foi corrigido.

**1 falso-positivo** do subagent (assimetria `splits`/`split` no Checkout é por design da API Asaas, não bug).

---

## 🔴 Bloqueantes — todos corrigidos

### B-09 — `EscrowConfig` / `SaveEscrowConfigRequest` fields errados
**Arquivo:** [Models/Escrow/EscrowConfig.cs](Codout.Apis.Asaas/Models/Escrow/EscrowConfig.cs), [Models/Escrow/SaveEscrowConfigRequest.cs](Codout.Apis.Asaas/Models/Escrow/SaveEscrowConfigRequest.cs)
- Campo era `DaysUntilExpire`, API documenta **`DaysToExpire`** (obrigatório).
- Faltava `IsFeePayer`.

### B-10 — `Escrow` (response): `Status` e `FinishReason` deveriam ser enums
**Arquivo:** [Models/Escrow/Escrow.cs](Codout.Apis.Asaas/Models/Escrow/Escrow.cs)
- `Status`: enum `EscrowStatus` (ACTIVE/DONE)
- `FinishReason`: enum `EscrowFinishReason` (CHARGEBACK/EXPIRED/INSUFFICIENT_BALANCE/PAYMENT_REFUNDED/REQUESTED_BY_CUSTOMER/CUSTOMER_CONFIG_DISABLED)

### B-11 — `FinishPaymentEscrow` retorna `Payment`, não `Escrow`
**Arquivo:** [Managers/EscrowManager.cs](Codout.Apis.Asaas/Managers/EscrowManager.cs)
- API retorna `PaymentGetResponseDTO`, body é objeto vazio `{}`.
- Removido `FinishEscrowRequest` (não existe na API).

### B-12 — `PixRecurringTransaction` muito errado
**Arquivo:** [Models/PixRecurring/PixRecurringTransaction.cs](Codout.Apis.Asaas/Models/PixRecurring/PixRecurringTransaction.cs)
- Campos inventados removidos: `Description`, `Periodicity`, `EndDate`, `DateCreated`.
- Adicionados conforme API: `Origin` (enum), `Frequency` (enum, não Periodicity!), `Quantity`, `FinishDate`, `CanBeCancelled`, `ExternalAccount`.
- Status convertido para enum `PixRecurringStatus` (5 valores).

### B-13 — `PixRecurringItem` muito errado
**Arquivo:** [Models/PixRecurring/PixRecurringItem.cs](Codout.Apis.Asaas/Models/PixRecurring/PixRecurringItem.cs)
- Campos inventados removidos: `EffectiveDate`, `Description`.
- Adicionados: `CanBeCancelled`, `RecurrenceNumber`, `Quantity`, `RefusalReasonDescription`, `ExternalAccount`.
- Status convertido para enum `PixRecurringItemStatus`.

### B-14 — `ListItems` (Pix Recurring) usa envelope `{data:[...]}`
**Arquivo:** [Managers/PixRecurringManager.cs](Codout.Apis.Asaas/Managers/PixRecurringManager.cs)
- A API retorna `{data:[...]}` sem `hasMore/totalCount/limit/offset`.
- Retorno mudou de `ResponseList<PixRecurringItem>` para `ResponseObject<PixRecurringItemsResponse>`.
- Mesma issue do B-07 (AccountDocument).

### B-15 / I-12 — `RequestParameters.Add(bool?)` serializava como `"True"`/`"False"`
**Arquivo:** [Core/RequestParameters.cs](Codout.Apis.Asaas/Core/RequestParameters.cs)
- `bool.ToString()` retorna `"True"` (PascalCase). API Asaas espera `"true"`/`"false"` (lowercase).
- Sem o fix, filtros booleanos (`WebhookListFilter.Enabled`, `PaymentListFilter.Anticipated`, etc) eram silenciosamente ignorados pela API.
- Tests `RequestParametersTests.Add_BoolTrueValue_AddsStringTrue` e `_Add_BoolFalseValue_AddsStringFalse` assertavam o comportamento errado — corrigidos.

---

## 🟡 Importantes — corrigidos

### I-11 — `Subscription.Deleted` é `bool` (não `bool?`)
**Arquivo:** [Models/Subscription/Subscription.cs:43](Codout.Apis.Asaas/Models/Subscription/Subscription.cs#L43)
- Customer.Deleted, Payment.Deleted, Installment.Deleted foram convertidos durante o PR — Subscription ficou para trás. Convertido para `bool?`.

### I-10 — README com ~12 referências stale ao 2.x
**Arquivo:** [README.md](README.md)
- `WasSucessfull()` → `WasSuccessful()` (4 ocorrências)
- Contagens: 20 managers / 400 testes → 27 managers / 492 testes
- `Transfer.Execute` → `TransferToAsaasAccount` / `TransferToBankAccount`
- `ReceivableAnticipation` → `Anticipation`
- `CustomerFiscalInfo` → `FiscalInfo`
- `MyAccount.Find` → `GetCommercialInfo`
- `Finance.Balance()` → `GetBalance()` (retorna objeto `{ Value }`)
- `Invoice.ListMunicipalServices` → `FiscalInfo.ListServices`
- Tabela de managers expandida para 27 entradas
- Adicionados exemplos de Webhook (CRUD), Sandbox, novo Transfer

---

## 🟢 Polimento — adiado

### P-04 — Sem testes de serialização para 10 domínios novos
**Arquivo:** [Tests/Serialization/SerializationTests.cs](Codout.Apis.Asaas.Tests/Serialization/SerializationTests.cs)
- Falta cobertura específica de serialização para: Chargeback, Escrow, Checkout, MobilePhoneRecharge, PixAutomatic, PixRecurring, AccountDocument, PaymentLimits, PaymentBillingInfo, SimulatedPayment.
- Enums já são exercitados indiretamente nos manager tests (ex: `ChargebackManagerTests` assert `ChargebackStatus.REQUESTED` from JSON).
- **Não bloqueia o release**. Recomendado como follow-up.

---

## Falsos-positivos do subagent (não-issues)

### C-01 (subagent) — `CreateCheckoutRequest.Splits` (plural) vs `Checkout.Split` (singular)
**Verdict:** ✅ Código atual está correto. O Asaas é assimétrico por design:
- `CheckoutSessionSaveRequestDTO.splits` (plural)
- `CheckoutSessionResponseDTO.split` (singular)

Adicionado comentário no model para evitar "correções" futuras erradas:
```csharp
// Asaas usa "splits" (plural) no request e "split" (singular) no response.
// Nao "corrigir" essa assimetria — a API e assim por design.
public List<CheckoutSplit> Splits { get; set; } = [];
```

---

## Verificações que confirmaram não-issues

| Verificação | Resultado |
|---|---|
| `_settings` migrado para `Settings` em todos os arquivos | ✅ Zero ocorrências de `_settings` remanescentes |
| `ReceivableAnticipation` rename completo | ✅ Apenas em docs (README+CHANGELOG), zero em .cs |
| File splitting do I-07 — duplicatas? | ✅ Nenhuma duplicata |
| Sandbox guard tests exercitam de fato `EnsureSandbox()`? | ✅ Sim — guard roda antes de `BuildHttpClient` |
| `WasSuccessful()` nos testes — ainda alguma chamada antiga? | ✅ Zero `WasSucessfull` em .cs (exceto o `[Obsolete]` alias) |
| `InternalsVisibleTo` para `SerializationTests`? | ✅ Configurado em csproj |
| `AnticipationStatusExtension` colocado junto do enum no mesmo arquivo? | ✅ Padrão pré-existente (BillPaymentStatus, PersonType, BillingType seguem o mesmo padrão) — não foi alvo do I-07 |
| `protected readonly Settings` naming compliance? | ✅ PascalCase é convenção .NET para protected field |

---

## Estado final pós-REVIEW2

- ✅ **492 testes** passando em Release
- ✅ **Zero warnings CS0618** (`WasSucessfull` obsoleto não é mais chamado)
- ✅ **Zero Newtonsoft** — só `System.Text.Json` (confirmado)
- ✅ **Zero ocorrências de `True`/`False` (capital)** em query strings serializadas
- ✅ **Schemas verificados via MCP:** Customer, Payment, Subscription (parcial), FiscalInfo, Finance, Webhook, Anticipation (parcial), Pix, PixAutomatic, PixRecurring, MyAccount, AsaasAccount (parcial), Escrow, Checkout, MobilePhoneRecharge, Chargeback
- ⚠️ **Schemas NÃO verificados a fundo** (escopo limitado, modelos pré-existentes): Invoice detalhado, PaymentDunning detalhado, CreditBureauReport, BillPayment (parcial), CustomerFiscalInfo legacy (já renomeado para FiscalInfo)

---

## Branch state

```
audit/asaas-api-conformance
~42 commits acima de master
```

Próximo passo: `git push -u origin audit/asaas-api-conformance` quando aprovado.
