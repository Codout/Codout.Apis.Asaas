using System;

namespace Codout.Apis.Asaas.Models.Pix;

public class PixOriginalTransaction
{
    public string Id { get; set; }
    public string EndToEndIdentifier { get; set; }
    public decimal? Value { get; set; }
    public DateTime? EffectiveDate { get; set; }
}
