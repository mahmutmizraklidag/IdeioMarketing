using IdeioMarketing.MarketingFeature.Entities;

namespace IdeioMarketing.MarketingFeature.Data
{
    public static class MarketingSeedData
    {
        private static readonly DateTime SeedStamp = new(2026, 7, 8, 0, 0, 0, DateTimeKind.Unspecified);

        public static MarketingStage[] Stages =>
        [
            new() { Id = 1, Key = "new", Label = "Yeni", Color = "#9AA0A6", SortOrder = 1 },
            new() { Id = 2, Key = "contacted", Label = "Görüşme", Color = "#4FA0E6", SortOrder = 2 },
            new() { Id = 3, Key = "proposal", Label = "Teklif", Color = "#FF7A3C", SortOrder = 3 },
            new() { Id = 4, Key = "negotiation", Label = "Müzakere", Color = "#E6A93C", SortOrder = 4 },
            new() { Id = 5, Key = "won", Label = "Satış Tamamlandı", Color = "#39C07A", SortOrder = 5 },
            new() { Id = 6, Key = "lost", Label = "Kaybedildi", Color = "#E0544E", SortOrder = 6 }
        ];

        public static MarketingSource[] Sources =>
        [
            new() { Id = 1, Name = "Referans", SortOrder = 1 },
            new() { Id = 2, Name = "Instagram", SortOrder = 2 },
            new() { Id = 3, Name = "Google", SortOrder = 3 },
            new() { Id = 4, Name = "Web Sitesi", SortOrder = 4 },
            new() { Id = 5, Name = "LinkedIn", SortOrder = 5 },
            new() { Id = 6, Name = "Soğuk Arama", SortOrder = 6 },
            new() { Id = 7, Name = "Fuar / Etkinlik", SortOrder = 7 }
        ];

        public static MarketingLeadStatus[] Statuses =>
        [
            new() { Id = 1, Key = "duzenli", Label = "Düzenli İş", IsRecurring = true, SortOrder = 1 },
            new() { Id = 2, Key = "dis", Label = "Dış İş", IsRecurring = false, SortOrder = 2 }
        ];

        public static MarketingLeadTemperature[] Temperatures =>
        [
            new() { Id = 1, Key = "sicak", Label = "Sıcak", Color = "#FF7A3C", SoftColor = "rgba(255,122,60,.16)", SortOrder = 1 },
            new() { Id = 2, Key = "soguk", Label = "Soğuk", Color = "#4FA0E6", SoftColor = "rgba(79,160,230,.16)", SortOrder = 2 }
        ];

        public static MarketingOwner[] Owners =>
        [
            new() { Id = 1, Name = "Ege", Color = "#FF7A3C", SortOrder = 1 },
            new() { Id = 2, Name = "Fırat", Color = "#E0544E", SortOrder = 2 },
            new() { Id = 3, Name = "Emre", Color = "#4FA0E6", SortOrder = 3 },
            new() { Id = 4, Name = "Göksel", Color = "#39C07A", SortOrder = 4 }
        ];

        public static MarketingLead[] Leads => LeadSpecs
            .Select(x => new MarketingLead
            {
                Id = x.Id,
                ExternalId = x.ExternalId,
                Company = x.Company,
                Contact = string.Empty,
                Email = string.Empty,
                SourceId = 1,
                StatusId = x.StatusId,
                TemperatureId = x.TemperatureId,
                StageId = x.StageId,
                Value = x.Value,
                Date = x.Date,
                Note = string.Empty,
                SortOrder = x.Id,
                CreatedAt = SeedStamp,
                UpdatedAt = SeedStamp
            })
            .ToArray();

        public static MarketingLeadOwner[] LeadOwners => LeadSpecs
            .SelectMany(x => x.Owners.Split('|').Select((owner, index) => new MarketingLeadOwner
            {
                MarketingLeadId = x.Id,
                MarketingOwnerId = OwnerId(owner),
                SortOrder = index + 1
            }))
            .ToArray();

        private static int OwnerId(string owner) => owner switch
        {
            "Ege" => 1,
            "Fırat" => 2,
            "Emre" => 3,
            "Göksel" => 4,
            _ => throw new InvalidOperationException($"Bilinmeyen marketing sorumlusu: {owner}")
        };

        private static readonly LeadSpec[] LeadSpecs =
        [
            new(1, "c1", "Alaş İnşaat", "Ege|Emre", 110000m, new DateTime(2026, 5, 15), 1, 1, 5),
            new(2, "c2", "Bilişim Garajı", "Ege|Fırat", 102000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(3, "c3", "Pro Estetik Diş Kliniği", "Emre|Fırat", 92500m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(4, "c4", "Gerilim Enerji", "Fırat", 79800m, new DateTime(2026, 3, 15), 1, 1, 5),
            new(5, "c5", "Dr. Çağlar İmançer", "Emre", 75000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(6, "c6", "Güzelbahçe Fen Bilimleri Koleji", "Göksel", 75000m, new DateTime(2026, 3, 15), 1, 1, 5),
            new(7, "c7", "Dr. Muzaffer Tunç", "Emre", 65000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(8, "c8", "Pal Mühendislik", "Ege", 62700m, new DateTime(2026, 4, 15), 1, 1, 5),
            new(9, "c9", "Boğatepe Köy Mandırası", "Fırat", 50000m, new DateTime(2026, 2, 15), 1, 1, 5),
            new(10, "c10", "Dr. Özlem Gürbüz Nazlı", "Göksel", 45600m, new DateTime(2026, 2, 15), 1, 1, 5),
            new(11, "c11", "Catchupper", "Ege", 45600m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(12, "c12", "Ercey Design", "Emre", 41040m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(13, "c13", "Dr. Mehmet Sucubaşı", "Fırat", 40000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(14, "c14", "Prof. Dr. Törün Özer", "Fırat", 40000m, new DateTime(2026, 3, 15), 1, 1, 5),
            new(15, "c15", "Esbay OSGB", "Fırat", 40000m, new DateTime(2026, 2, 15), 1, 1, 5),
            new(16, "c16", "Dr. Sevil Tunabayoğlu", "Emre", 36000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(17, "c17", "Berkay & Koray Nazlı", "Göksel", 35000m, new DateTime(2026, 3, 15), 1, 1, 5),
            new(18, "c18", "Dr. Süleyman & Dr. Sezin", "Emre", 56000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(19, "c19", "Uzm. Dr. Sibel Karkaç", "Emre", 25000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(20, "c20", "Ossi Hair", "Göksel", 20000m, new DateTime(2025, 12, 1), 1, 1, 5),
            new(21, "c21", "Rota Home Bellona", "Ege", 55500m, new DateTime(2026, 4, 15), 1, 1, 5),
            new(22, "d1", "Fikret Mungan", "Fırat", 45600m, new DateTime(2026, 1, 15), 2, 1, 5),
            new(23, "d2", "Bombacı Zeydan", "Ege|Fırat", 30000m, new DateTime(2026, 1, 15), 2, 1, 5),
            new(24, "d3", "Özlem Gürbüz Nazlı", "Göksel", 45600m, new DateTime(2026, 2, 15), 2, 1, 5),
            new(25, "d4", "Dijital Futbol Akademi", "Fırat", 300000m, new DateTime(2026, 2, 15), 2, 1, 5),
            new(26, "d5", "Bilişim Garajı (Ekstralar)", "Ege", 57000m, new DateTime(2026, 2, 15), 2, 1, 5),
            new(27, "d6", "İlkim Makina", "Fırat", 20000m, new DateTime(2026, 2, 15), 2, 1, 5),
            new(28, "d7", "Bombacı Zeydan", "Fırat", 10000m, new DateTime(2026, 3, 15), 2, 1, 5),
            new(29, "d8", "Jaggermaister", "Emre", 12000m, new DateTime(2026, 3, 15), 2, 1, 5),
            new(30, "d9", "İlkim Mühendislik", "Fırat", 151000m, new DateTime(2026, 3, 15), 2, 1, 5),
            new(31, "d10", "Günseli Uyar", "Ege", 10000m, new DateTime(2026, 3, 15), 2, 1, 5),
            new(32, "d11", "Gold Performans", "Fırat", 207000m, new DateTime(2026, 4, 15), 2, 1, 5),
            new(33, "d12", "Oben Home", "Emre", 80000m, new DateTime(2026, 4, 15), 2, 1, 5),
            new(34, "d13", "Viora Coffe", "Göksel", 6840m, new DateTime(2026, 4, 15), 2, 1, 5),
            new(35, "d14", "Pedalanka", "Fırat", 15000m, new DateTime(2026, 4, 15), 2, 1, 5),
            new(36, "d15", "Bilet Point", "Göksel", 29000m, new DateTime(2026, 4, 15), 2, 1, 5),
            new(37, "d16", "İlkim Makina", "Fırat", 200000m, new DateTime(2026, 4, 15), 2, 1, 5),
            new(38, "d17", "Zişan Cin", "Fırat", 40000m, new DateTime(2026, 4, 15), 2, 1, 5),
            new(39, "d18", "Gold Performans", "Fırat", 163000m, new DateTime(2026, 5, 15), 2, 1, 5),
            new(40, "d19", "Pal Mühendislik", "Ege", 30000m, new DateTime(2026, 5, 15), 2, 1, 5),
            new(41, "d20", "Oha Patch", "Emre", 54000m, new DateTime(2026, 5, 15), 2, 1, 5),
            new(42, "d21", "Keep Risen", "Fırat", 50000m, new DateTime(2026, 6, 15), 2, 1, 5),
            new(43, "d22", "Proline", "Göksel", 85500m, new DateTime(2026, 6, 15), 2, 1, 5),
            new(44, "d23", "Zero1", "Göksel", 34200m, new DateTime(2026, 6, 15), 2, 1, 5),
            new(45, "d24", "Welness", "Göksel", 25000m, new DateTime(2026, 6, 15), 2, 1, 5),
            new(46, "d25", "İzmir Avukat Hareketi (İZAH)", "Ege|Fırat", 370000m, new DateTime(2026, 6, 15), 2, 1, 5),
            new(47, "d26", "Proline (Katalog)", "Göksel", 57000m, new DateTime(2026, 6, 15), 2, 1, 5),
            new(48, "p1", "C3 Teknoloji", "Ege", 0m, new DateTime(2026, 6, 15), 1, 1, 4),
            new(49, "p2", "Bilimsev Koleji", "Ege", 0m, new DateTime(2026, 6, 15), 1, 1, 3),
            new(50, "p3", "Sanat Garajı", "Fırat", 0m, new DateTime(2026, 6, 15), 1, 1, 2),
            new(51, "p4", "İzmir Avukat Hareketi", "Ege", 0m, new DateTime(2026, 6, 15), 1, 1, 4),
            new(52, "p5", "Lazarus", "Emre", 0m, new DateTime(2026, 6, 15), 1, 1, 3),
            new(53, "p6", "Oha Yatch", "Emre", 0m, new DateTime(2026, 6, 15), 1, 1, 4),
            new(54, "p7", "Boatfinder", "Emre", 0m, new DateTime(2026, 6, 15), 2, 1, 4),
            new(55, "p8", "İzmir Cerrahi Onkoloji", "Göksel", 0m, new DateTime(2026, 6, 15), 1, 1, 3),
            new(56, "p9", "Wellnes", "Göksel", 0m, new DateTime(2026, 6, 15), 1, 1, 3),
            new(57, "p10", "SANGROW", "Ege", 0m, new DateTime(2026, 6, 15), 1, 2, 1),
            new(58, "p11", "Netforce", "Ege", 0m, new DateTime(2026, 6, 15), 1, 2, 1),
            new(59, "p12", "Metod Koleji", "Ege", 0m, new DateTime(2026, 6, 15), 1, 2, 1),
            new(60, "p13", "Proline", "Göksel", 0m, new DateTime(2026, 6, 15), 1, 2, 2),
            new(61, "p14", "İlkim Makina", "Göksel", 0m, new DateTime(2026, 6, 15), 1, 2, 2),
            new(62, "p15", "4R Mühendislik", "Ege", 0m, new DateTime(2026, 6, 15), 1, 1, 2),
            new(63, "p16", "Arkas Holding", "Ege", 0m, new DateTime(2026, 6, 15), 2, 1, 3),
            new(64, "p17", "Cour de Lion", "Emre", 0m, new DateTime(2026, 6, 15), 1, 1, 2),
            new(65, "x1", "The Vets Hub", "Emre", 20000m, new DateTime(2026, 1, 20), 1, 1, 6),
            new(66, "x2", "Zero1", "Göksel", 51300m, new DateTime(2026, 1, 20), 1, 1, 6),
            new(67, "x3", "Pekari", "Fırat", 28500m, new DateTime(2026, 1, 20), 1, 1, 6),
            new(68, "x4", "Funce Medical", "Emre", 45000m, new DateTime(2026, 1, 20), 1, 1, 6),
            new(69, "x5", "Baskın Kocabaş", "Emre", 35000m, new DateTime(2026, 1, 20), 1, 1, 6),
            new(70, "x6", "Tolga Bıçakcı", "Emre", 35000m, new DateTime(2026, 1, 20), 1, 1, 6),
            new(71, "x7", "Çiler Ezgi", "Fırat", 57000m, new DateTime(2026, 2, 20), 1, 1, 6),
            new(72, "x8", "Bjorn Coffe", "Fırat", 34200m, new DateTime(2026, 2, 20), 1, 1, 6),
            new(73, "x9", "Kaan Akacun", "Emre", 33000m, new DateTime(2026, 2, 20), 1, 1, 6),
            new(74, "x10", "Dino Pizza", "Emre", 30000m, new DateTime(2026, 2, 20), 1, 1, 6),
            new(75, "x11", "Saygın Health", "Emre", 35000m, new DateTime(2026, 2, 20), 1, 1, 6),
            new(76, "x12", "Ecem Cantürk Nazlı", "Göksel", 51300m, new DateTime(2026, 3, 20), 1, 1, 6),
            new(77, "x13", "Zişan Cin", "Fırat", 34200m, new DateTime(2026, 3, 20), 1, 1, 6),
            new(78, "x14", "Hakkı Kurt", "Göksel", 40000m, new DateTime(2026, 5, 20), 1, 1, 6),
            new(79, "x15", "Bilet Points", "Göksel", 80000m, new DateTime(2026, 5, 20), 1, 1, 6),
            new(80, "x16", "Venettia Pizza", "Ege", 25000m, new DateTime(2026, 6, 20), 1, 1, 6)
        ];

        private sealed record LeadSpec(
            int Id,
            string ExternalId,
            string Company,
            string Owners,
            decimal Value,
            DateTime Date,
            int StatusId,
            int TemperatureId,
            int StageId);
    }
}
