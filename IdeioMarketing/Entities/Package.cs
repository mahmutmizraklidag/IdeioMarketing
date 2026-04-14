using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int? Piece { get; set; }
        public bool IsPiece { get; set; }
        public bool IsOneTime { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<PackageFeatures> PackageFeatures { get; set; }
        public Package()
        {
            PackageFeatures = new List<PackageFeatures>();
        }

    }
}
