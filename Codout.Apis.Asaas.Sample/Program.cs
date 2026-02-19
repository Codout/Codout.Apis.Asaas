using Codout.Apis.Asaas;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Customer;
using Codout.Apis.Asaas.Models.Payment;

ApiSettings apiSettings = new ApiSettings("YOUR_ACCESS_TOKEN", "Codout", AsaasEnvironment.SANDBOX);

AsaasApi asaasApi = new AsaasApi(apiSettings);

ResponseObject<Customer> customerResponse = await asaasApi.Customer.Find("cus_13bFHumeyglN");

if (customerResponse.WasSucessfull())
{
    Customer customer = customerResponse.Data;

    ResponseObject<Payment> paymentResponse = await asaasApi.Payment.Create(new CreatePaymentRequest()
    {
        CustomerId = customer.Id,
        BillingType = BillingType.BOLETO,
        Value = 32.55M,
        DueDate = DateTime.Parse("12/12/2020")
    });
}