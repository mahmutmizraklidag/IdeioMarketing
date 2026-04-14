using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class SubPackage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int?  Piece { get; set; }
        public bool IsPiece { get; set; }
        public bool IsOneTime { get; set; } 
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; } = null;

        public virtual ICollection<PackageFeatures> PackageFeatures { get; set; }
        public SubPackage()
        {
            PackageFeatures = new List<PackageFeatures>();
        }
    }
}
