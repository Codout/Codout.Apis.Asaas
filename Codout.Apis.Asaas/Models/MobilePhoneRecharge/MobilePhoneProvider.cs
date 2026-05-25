using System.Collections.Generic;

namespace Codout.Apis.Asaas.Models.MobilePhoneRecharge;

public class MobilePhoneProvider
{
    public string Name { get; set; }
    public List<MobilePhoneProviderValue> Values { get; set; } = [];
}
