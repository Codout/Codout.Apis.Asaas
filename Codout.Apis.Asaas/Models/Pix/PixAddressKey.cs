using System;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Models.Pix
{
    public class PixAddressKey
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public PixAddressKeyType Type { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
