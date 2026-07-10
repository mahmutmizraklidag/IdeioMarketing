using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class SubPackage
    {
        // Alt paketin benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Teklif ekranında gösterilen alt paket adıdır.
        public string Name { get; set; }

        // Alt paketin liste veya satış fiyatıdır.
        public string Price { get; set; }

        // Adetli alt paketlerde varsayılan veya seçilen adet bilgisidir.
        public int?  Piece { get; set; }

        // Alt paketin adet üzerinden hesaplanıp hesaplanmadığını belirtir.
        public bool IsPiece { get; set; }

        // Alt paketin tek seferlik ücret olarak değerlendirilip değerlendirilmediğini belirtir.
        public bool IsOneTime { get; set; } 

        // Alt paketin bağlı olduğu kategori kaydının numarasıdır.
        public int? CategoryId { get; set; }

        // Alt paketin bağlı olduğu kategori bilgisidir.
        public virtual Category? Category { get; set; } = null;

        // Alt pakete ait özellik açıklamalarını tutar.
        public virtual ICollection<PackageFeatures> PackageFeatures { get; set; }
        public SubPackage()
        {
            PackageFeatures = new List<PackageFeatures>();
        }
    }
}
