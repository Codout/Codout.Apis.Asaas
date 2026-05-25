using System;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Models.Pix;

public class PixAddressKey
{
    public string Id { get; set; }
    public string Key { get; set; }
    public PixAddressKeyType Type { get; set; }
    public PixAddressKeyStatus Status { get; set; }
    public DateTime? DateCreated { get; set; }
    public bool? CanBeDeleted { get; set; }
    public string CannotBeDeletedReason { get; set; }
    public PixAddressKeyQrCode QrCode { get; set; }
}
