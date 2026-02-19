using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.Common
{
    public class AsaasFile : IAsaasFile
    {
        public string FileName { get; set; }

        public byte[] FileContent { get; set; }
    }
}
