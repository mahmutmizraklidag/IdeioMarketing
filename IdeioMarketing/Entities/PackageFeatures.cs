namespace IdeioMarketing.Entities
{
    public class PackageFeatures
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? PackageId { get; set; }
        public virtual Package? Package { get; set; }
        public int? SubPackageId { get; set; }
        public virtual SubPackage? SubPackage { get; set; }
    }
}
