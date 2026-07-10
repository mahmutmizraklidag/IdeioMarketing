using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class OfferRecordItem
    {
        // Teklif kaleminin benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Kalemin bağlı olduğu teklif kaydının numarasıdır.
        public int OfferRecordId { get; set; }

        // Kalemin bağlı olduğu teklif kaydıdır.
        public virtual OfferRecord? OfferRecord { get; set; }

        // Kalemin paket mi alt paket mi olduğunu belirtir.
        [MaxLength(30)]
        public string ItemType { get; set; } = string.Empty; // package / subpackage

        // Kalemin seçildiği kaynak paket veya alt paket kaydının numarasıdır.
        public int SourceItemId { get; set; }

        // Kalemin bağlı olduğu kategori kaydının numarasıdır.
        public int CategoryId { get; set; }

        // Teklif oluşturulduğu anda kategori adını saklar.
        [MaxLength(200)]
        public string CategoryName { get; set; } = string.Empty;

        // Teklif kaleminin ekranda gösterilen adıdır.
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        // Teklif kaleminin hesaplama veya kaynak veriden gelen ham adıdır.
        [MaxLength(250)]
        public string RawName { get; set; } = string.Empty;

        // Kalemin birim fiyatıdır.
        public decimal UnitPrice { get; set; }

        // Kalem için seçilen adet miktarıdır.
        public int Quantity { get; set; }

        // Kalemin adet ve birim fiyata göre hesaplanan toplam tutarıdır.
        public decimal TotalPrice { get; set; }

        // Kalemin tek seferlik ücret olarak değerlendirilip değerlendirilmediğini belirtir.
        public bool IsOneTime { get; set; }

        // Kalemin adet üzerinden hesaplanıp hesaplanmadığını belirtir.
        public bool IsPiece { get; set; }
    }
}
