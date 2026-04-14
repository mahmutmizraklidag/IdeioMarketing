using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class OfferRecordItem
    {
        public int Id { get; set; }

        public int OfferRecordId { get; set; }
        public virtual OfferRecord? OfferRecord { get; set; }

        [MaxLength(30)]
        public string ItemType { get; set; } = string.Empty; // package / subpackage

        public int SourceItemId { get; set; }
        public int CategoryId { get; set; }

        [MaxLength(200)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string RawName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public bool IsOneTime { get; set; }
        public bool IsPiece { get; set; }
    }
}