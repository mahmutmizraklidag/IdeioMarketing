namespace IdeioMarketing.Entities
{
    public class PaymentPlan
    {
        // Ödeme planının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Ödeme planının ekranda gösterilen adıdır.
        public string Name { get; set; }

        // Ödeme planındaki toplam taksit sayısıdır.
        public int NumberOfInstallments { get; set; }

        // Ödeme planı için uygulanacak indirim oranıdır.
        public int DiscountRate { get; set; }
    }
}
