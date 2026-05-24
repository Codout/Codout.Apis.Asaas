using System;
using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Payment;

namespace Codout.Apis.Asaas.Managers;

public class SandboxManager(ApiSettings settings) : BaseManager(settings)
{
    private const string SandboxRoute = "/sandbox";

    private void EnsureSandbox()
    {
        if (Settings.AsaasEnvironment.IsProduction())
        {
            throw new InvalidOperationException(
                "SandboxManager so pode ser usado em AsaasEnvironment.SANDBOX. " +
                "Os endpoints /v3/sandbox/* nao existem em producao.");
        }
    }

    public async Task<ResponseObject<object>> ApproveAccount()
    {
        EnsureSandbox();
        var route = $"{SandboxRoute}/myAccount/approve";
        return await PostAsync<object>(route, new RequestParameters());
    }

    public async Task<ResponseObject<Payment>> ConfirmPayment(string paymentId)
    {
        EnsureSandbox();
        var route = $"{SandboxRoute}/payment/{paymentId}/confirm";
        return await PostAsync<Payment>(route, new RequestParameters());
    }

    public async Task<ResponseObject<Payment>> ForceOverdue(string paymentId)
    {
        EnsureSandbox();
        var route = $"{SandboxRoute}/payment/{paymentId}/overdue";
        return await PostAsync<Payment>(route, new RequestParameters());
    }
}
