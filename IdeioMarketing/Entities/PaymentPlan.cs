namespace IdeioMarketing.Entities
{
    public class PaymentPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfInstallments { get; set; }
        public int DiscountRate { get; set; }
    }
}