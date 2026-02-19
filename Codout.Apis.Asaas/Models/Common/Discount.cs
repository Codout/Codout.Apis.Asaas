using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Common
{
    public class Discount
    {
        public decimal Value { get; set; }

        public int DueDateLimitDays { get; set; }

        public DiscountType Type { get; set; }
    }
}
