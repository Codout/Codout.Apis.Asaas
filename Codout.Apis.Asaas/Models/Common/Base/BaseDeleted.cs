namespace Codout.Apis.Asaas.Models.Common.Base
{
    public abstract class BaseDeleted
    {
        public string Id { get; set; }

        public bool Deleted { get; set; }
    }
}