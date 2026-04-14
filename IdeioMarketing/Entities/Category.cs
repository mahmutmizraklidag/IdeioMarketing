using Microsoft.AspNetCore.Components.Web;

namespace IdeioMarketing.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPackageMultiSelected { get; set; }
        public bool IsSubPackageMultiSelected { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<SubPackage> SubPackages { get; set; }
        public Category() 
        { 
            Packages = new List<Package>();
            SubPackages = new List<SubPackage>();
        }
    }
}
