using Microsoft.AspNetCore.Components.Web;

namespace IdeioMarketing.Entities
{
    public class Category
    {
        // Kategorinin benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Teklif ekranında gösterilen kategori adıdır.
        public string Name { get; set; }

        // Bu kategoride birden fazla paket seçilip seçilemeyeceğini belirtir.
        public bool IsPackageMultiSelected { get; set; }

        // Bu kategoride birden fazla alt paket seçilip seçilemeyeceğini belirtir.
        public bool IsSubPackageMultiSelected { get; set; }

        // Kategoriye bağlı paketleri tutar.
        public virtual ICollection<Package> Packages { get; set; }

        // Kategoriye bağlı alt paketleri tutar.
        public virtual ICollection<SubPackage> SubPackages { get; set; }
        public Category() 
        { 
            Packages = new List<Package>();
            SubPackages = new List<SubPackage>();
        }
    }
}
