namespace IdeioMarketing.Entities
{
    public class PackageFeatures
    {
        // Özellik kaydının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Paket veya alt paket için gösterilen özellik metnidir.
        public string Title { get; set; }

        // Özelliğin bağlı olduğu paket kaydının numarasıdır.
        public int? PackageId { get; set; }

        // Özelliğin bağlı olduğu paket bilgisidir.
        public virtual Package? Package { get; set; }

        // Özelliğin bağlı olduğu alt paket kaydının numarasıdır.
        public int? SubPackageId { get; set; }

        // Özelliğin bağlı olduğu alt paket bilgisidir.
        public virtual SubPackage? SubPackage { get; set; }
    }
}
