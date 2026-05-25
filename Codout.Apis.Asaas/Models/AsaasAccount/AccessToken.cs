using System;

namespace Codout.Apis.Asaas.Models.AsaasAccount;

public class AccessToken
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public string ApiKey { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool Enabled { get; set; }
}
