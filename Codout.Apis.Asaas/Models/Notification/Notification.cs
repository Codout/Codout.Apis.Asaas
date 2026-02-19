namespace Codout.Apis.Asaas.Models.Notification
{
    public class Notification
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public bool Enabled { get; set; }
        public bool EmailEnabledForProvider { get; set; }
        public bool SmsEnabledForProvider { get; set; }
        public bool EmailEnabledForCustomer { get; set; }
        public bool SmsEnabledForCustomer { get; set; }
        public bool PhoneCallEnabledForCustomer { get; set; }
        public bool WhatsappEnabledForCustomer { get; set; }
        public int? ScheduleOffset { get; set; }
    }
}
