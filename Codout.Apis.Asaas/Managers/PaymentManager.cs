using System;
using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Base;
using Codout.Apis.Asaas.Models.Payment;

namespace Codout.Apis.Asaas.Managers;

public class PaymentManager(ApiSettings settings) : BaseManager(settings)
{
    private const string PaymentsRoute = "/payments";

    public async Task<ResponseObject<Payment>> Create(CreatePaymentRequest requestObj)
    {
        return await PostAsync<Payment>(PaymentsRoute, requestObj);
    }

    public async Task<ResponseObject<Payment>> CreateWithCreditCard(CreatePaymentRequest requestObj)
    {
        return await PostAsync<Payment>($"{PaymentsRoute}/", requestObj);
    }

    public async Task<ResponseObject<Payment>> Find(string id)
    {
        var route = $"{PaymentsRoute}/{id}";
        return await GetAsync<Payment>(route);
    }

    public async Task<ResponseList<Payment>> List(int offset, int limit, PaymentListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<Payment>(PaymentsRoute, offset, limit, queryMap);
    }

    public async Task<ResponseObject<Payment>> Update(string paymentId, UpdatePaymentRequest requestObj)
    {
        var route = $"{PaymentsRoute}/{paymentId}";

        return await PutAsync<Payment>(route, requestObj);
    }

    public async Task<ResponseObject<DeletedPayment>> Delete(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}";
        return await DeleteAsync<DeletedPayment>(route);
    }

    public async Task<ResponseObject<Payment>> Restore(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/restore";

        return await PostAsync<Payment>(route, new RequestParameters());
    }

    public async Task<ResponseObject<Payment>> Refund(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/refund";
        return await PostAsync<Payment>(route, new RequestParameters());
    }

    public async Task<ResponseList<PaymentRefund>> ListRefunds(string paymentId, int offset, int limit)
    {
        var route = $"{PaymentsRoute}/{paymentId}/refunds";
        return await GetListAsync<PaymentRefund>(route, offset, limit);
    }

    public async Task<ResponseObject<Payment>> RefundBankSlip(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/bankSlip/refund";
        return await PostAsync<Payment>(route, new RequestParameters());
    }

    public async Task<ResponseObject<Payment>> ReceiveInCash(string paymentId, DateTime paymentDate, decimal value, bool notifyCustomer)
    {
        var route = $"{PaymentsRoute}/{paymentId}/receiveInCash";

        var parameters = new ReceiveInCashRequest
        {
            PaymentDate = paymentDate,
            Value = value,
            NotifyCustomer = notifyCustomer
        };

        return await PostAsync<Payment>(route, parameters);
    }

    public async Task<ResponseObject<Payment>> UndoReceivedInCash(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/undoReceivedInCash";
        return await PostAsync<Payment>(route, new RequestParameters());
    }

    public async Task<ResponseObject<BankSlipCode>> GetBankSlipBarCode(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/identificationField";

        return await GetAsync<BankSlipCode>(route);
    }

    public async Task<ResponseObject<PixQRCode>> GetPixQrCode(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/pixQrCode";

        return await GetAsync<PixQRCode>(route);
    }

    public async Task<ResponseObject<Payment>> CaptureAuthorizedPayment(string paymentId, CapturePaymentRequest requestObj)
    {
        var route = $"{PaymentsRoute}/{paymentId}/captureAuthorizedPayment";
        return await PostAsync<Payment>(route, requestObj);
    }

    public async Task<ResponseObject<Payment>> PayWithCreditCard(string paymentId, PayWithCreditCardRequest requestObj)
    {
        var route = $"{PaymentsRoute}/{paymentId}/payWithCreditCard";
        return await PostAsync<Payment>(route, requestObj);
    }

    public async Task<ResponseObject<PaymentBillingInfo>> GetBillingInfo(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/billingInfo";
        return await GetAsync<PaymentBillingInfo>(route);
    }

    public async Task<ResponseObject<PaymentViewingInfo>> GetViewingInfo(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/viewingInfo";
        return await GetAsync<PaymentViewingInfo>(route);
    }

    public async Task<ResponseObject<PaymentStatusInfo>> GetStatus(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/status";
        return await GetAsync<PaymentStatusInfo>(route);
    }

    public async Task<ResponseObject<SimulatedPayment>> Simulate(SimulatePaymentRequest requestObj)
    {
        var route = $"{PaymentsRoute}/simulate";
        return await PostAsync<SimulatedPayment>(route, requestObj);
    }

    public async Task<ResponseObject<PaymentLimits>> GetLimits()
    {
        var route = $"{PaymentsRoute}/limits";
        return await GetAsync<PaymentLimits>(route);
    }

    public async Task<ResponseObject<PaymentDocument>> UploadDocument(string paymentId, UploadPaymentDocumentRequest requestObj)
    {
        var route = $"{PaymentsRoute}/{paymentId}/documents";
        return await PostMultipartFormDataContentAsync<PaymentDocument>(route, requestObj);
    }

    public async Task<ResponseList<PaymentDocument>> ListDocuments(string paymentId, int offset, int limit)
    {
        var route = $"{PaymentsRoute}/{paymentId}/documents";
        return await GetListAsync<PaymentDocument>(route, offset, limit);
    }

    public async Task<ResponseObject<PaymentDocument>> FindDocument(string paymentId, string documentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/documents/{documentId}";
        return await GetAsync<PaymentDocument>(route);
    }

    public async Task<ResponseObject<PaymentDocument>> UpdateDocument(string paymentId, string documentId, UpdatePaymentDocumentRequest requestObj)
    {
        var route = $"{PaymentsRoute}/{paymentId}/documents/{documentId}";
        return await PutAsync<PaymentDocument>(route, requestObj);
    }

    public async Task<ResponseObject<BaseDeleted>> DeleteDocument(string paymentId, string documentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/documents/{documentId}";
        return await DeleteAsync<BaseDeleted>(route);
    }

    public async Task<ResponseList<PaymentSplitView>> ListPaidSplits(int offset, int limit)
    {
        var route = $"{PaymentsRoute}/splits/paid";
        return await GetListAsync<PaymentSplitView>(route, offset, limit);
    }

    public async Task<ResponseObject<PaymentSplitView>> FindPaidSplit(string splitId)
    {
        var route = $"{PaymentsRoute}/splits/paid/{splitId}";
        return await GetAsync<PaymentSplitView>(route);
    }

    public async Task<ResponseList<PaymentSplitView>> ListReceivedSplits(int offset, int limit)
    {
        var route = $"{PaymentsRoute}/splits/received";
        return await GetListAsync<PaymentSplitView>(route, offset, limit);
    }

    public async Task<ResponseObject<PaymentSplitView>> FindReceivedSplit(string splitId)
    {
        var route = $"{PaymentsRoute}/splits/received/{splitId}";
        return await GetAsync<PaymentSplitView>(route);
    }
}
