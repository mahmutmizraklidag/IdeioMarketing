using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class Package
    {
        // Paketin benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Teklif ekranında gösterilen paket adıdır.
        public string Name { get; set; }

        // Paketin liste veya satış fiyatıdır.
        public string Price { get; set; }

        // Adetli paketlerde varsayılan veya seçilen adet bilgisidir.
        public int? Piece { get; set; }

        // Paketin adet üzerinden hesaplanıp hesaplanmadığını belirtir.
        public bool IsPiece { get; set; }

        // Paketin tek seferlik ücret olarak değerlendirilip değerlendirilmediğini belirtir.
        public bool IsOneTime { get; set; }

        // Paketin bağlı olduğu kategori kaydının numarasıdır.
        public int? CategoryId { get; set; }

        // Paketin bağlı olduğu kategori bilgisidir.
        public virtual Category? Category { get; set; }

        // Pakete ait özellik açıklamalarını tutar.
        public virtual ICollection<PackageFeatures> PackageFeatures { get; set; }
        public Package()
        {
            PackageFeatures = new List<PackageFeatures>();
        }

    }
}
