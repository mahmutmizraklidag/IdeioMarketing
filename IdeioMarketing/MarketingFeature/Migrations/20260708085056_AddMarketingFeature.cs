using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdeioMarketing.MarketingFeature.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketingFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketingLeadStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingLeadStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingLeadTemperatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoftColor = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingLeadTemperatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingOwners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingOwners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingLeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TemperatureId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingLeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingLeads_MarketingLeadStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "MarketingLeadStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingLeads_MarketingLeadTemperatures_TemperatureId",
                        column: x => x.TemperatureId,
                        principalTable: "MarketingLeadTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingLeads_MarketingSources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "MarketingSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingLeads_MarketingStages_StageId",
                        column: x => x.StageId,
                        principalTable: "MarketingStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketingLeadOwners",
                columns: table => new
                {
                    MarketingLeadId = table.Column<int>(type: "int", nullable: false),
                    MarketingOwnerId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingLeadOwners", x => new { x.MarketingLeadId, x.MarketingOwnerId });
                    table.ForeignKey(
                        name: "FK_MarketingLeadOwners_MarketingLeads_MarketingLeadId",
                        column: x => x.MarketingLeadId,
                        principalTable: "MarketingLeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketingLeadOwners_MarketingOwners_MarketingOwnerId",
                        column: x => x.MarketingOwnerId,
                        principalTable: "MarketingOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MarketingLeadStatuses",
                columns: new[] { "Id", "IsRecurring", "Key", "Label", "SortOrder" },
                values: new object[,]
                {
                    { 1, true, "duzenli", "Düzenli İş", 1 },
                    { 2, false, "dis", "Dış İş", 2 }
                });

            migrationBuilder.InsertData(
                table: "MarketingLeadTemperatures",
                columns: new[] { "Id", "Color", "Key", "Label", "SoftColor", "SortOrder" },
                values: new object[,]
                {
                    { 1, "#FF7A3C", "sicak", "Sıcak", "rgba(255,122,60,.16)", 1 },
                    { 2, "#4FA0E6", "soguk", "Soğuk", "rgba(79,160,230,.16)", 2 }
                });

            migrationBuilder.InsertData(
                table: "MarketingOwners",
                columns: new[] { "Id", "Color", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "#FF7A3C", "Ege", 1 },
                    { 2, "#E0544E", "Fırat", 2 },
                    { 3, "#4FA0E6", "Emre", 3 },
                    { 4, "#39C07A", "Göksel", 4 }
                });

            migrationBuilder.InsertData(
                table: "MarketingSources",
                columns: new[] { "Id", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Referans", 1 },
                    { 2, "Instagram", 2 },
                    { 3, "Google", 3 },
                    { 4, "Web Sitesi", 4 },
                    { 5, "LinkedIn", 5 },
                    { 6, "Soğuk Arama", 6 },
                    { 7, "Fuar / Etkinlik", 7 }
                });

            migrationBuilder.InsertData(
                table: "MarketingStages",
                columns: new[] { "Id", "Color", "Key", "Label", "SortOrder" },
                values: new object[,]
                {
                    { 1, "#9AA0A6", "new", "Yeni", 1 },
                    { 2, "#4FA0E6", "contacted", "Görüşme", 2 },
                    { 3, "#FF7A3C", "proposal", "Teklif", 3 },
                    { 4, "#E6A93C", "negotiation", "Müzakere", 4 },
                    { 5, "#39C07A", "won", "Satış Tamamlandı", 5 },
                    { 6, "#E0544E", "lost", "Kaybedildi", 6 }
                });

            migrationBuilder.InsertData(
                table: "MarketingLeads",
                columns: new[] { "Id", "Company", "Contact", "CreatedAt", "Date", "Email", "ExternalId", "Note", "SortOrder", "SourceId", "StageId", "StatusId", "TemperatureId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, "Alaş İnşaat", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c1", "", 1, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 110000m },
                    { 2, "Bilişim Garajı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c2", "", 2, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 102000m },
                    { 3, "Pro Estetik Diş Kliniği", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c3", "", 3, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 92500m },
                    { 4, "Gerilim Enerji", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c4", "", 4, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 79800m },
                    { 5, "Dr. Çağlar İmançer", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c5", "", 5, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 75000m },
                    { 6, "Güzelbahçe Fen Bilimleri Koleji", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c6", "", 6, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 75000m },
                    { 7, "Dr. Muzaffer Tunç", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c7", "", 7, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 65000m },
                    { 8, "Pal Mühendislik", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c8", "", 8, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 62700m },
                    { 9, "Boğatepe Köy Mandırası", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c9", "", 9, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 50000m },
                    { 10, "Dr. Özlem Gürbüz Nazlı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c10", "", 10, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 45600m },
                    { 11, "Catchupper", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c11", "", 11, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 45600m },
                    { 12, "Ercey Design", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c12", "", 12, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 41040m },
                    { 13, "Dr. Mehmet Sucubaşı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c13", "", 13, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 40000m },
                    { 14, "Prof. Dr. Törün Özer", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c14", "", 14, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 40000m },
                    { 15, "Esbay OSGB", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c15", "", 15, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 40000m },
                    { 16, "Dr. Sevil Tunabayoğlu", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c16", "", 16, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 36000m },
                    { 17, "Berkay & Koray Nazlı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c17", "", 17, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 35000m },
                    { 18, "Dr. Süleyman & Dr. Sezin", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c18", "", 18, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 56000m },
                    { 19, "Uzm. Dr. Sibel Karkaç", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c19", "", 19, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 25000m },
                    { 20, "Ossi Hair", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c20", "", 20, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 20000m },
                    { 21, "Rota Home Bellona", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "c21", "", 21, 1, 5, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 55500m },
                    { 22, "Fikret Mungan", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d1", "", 22, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 45600m },
                    { 23, "Bombacı Zeydan", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d2", "", 23, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 30000m },
                    { 24, "Özlem Gürbüz Nazlı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d3", "", 24, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 45600m },
                    { 25, "Dijital Futbol Akademi", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d4", "", 25, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 300000m },
                    { 26, "Bilişim Garajı (Ekstralar)", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d5", "", 26, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 57000m },
                    { 27, "İlkim Makina", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d6", "", 27, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 20000m },
                    { 28, "Bombacı Zeydan", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d7", "", 28, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 10000m },
                    { 29, "Jaggermaister", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d8", "", 29, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 12000m },
                    { 30, "İlkim Mühendislik", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d9", "", 30, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 151000m },
                    { 31, "Günseli Uyar", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d10", "", 31, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 10000m },
                    { 32, "Gold Performans", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d11", "", 32, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 207000m },
                    { 33, "Oben Home", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d12", "", 33, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 80000m },
                    { 34, "Viora Coffe", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d13", "", 34, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 6840m },
                    { 35, "Pedalanka", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d14", "", 35, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 15000m },
                    { 36, "Bilet Point", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d15", "", 36, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 29000m },
                    { 37, "İlkim Makina", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d16", "", 37, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 200000m },
                    { 38, "Zişan Cin", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d17", "", 38, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 40000m },
                    { 39, "Gold Performans", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d18", "", 39, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 163000m },
                    { 40, "Pal Mühendislik", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d19", "", 40, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 30000m },
                    { 41, "Oha Patch", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d20", "", 41, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 54000m },
                    { 42, "Keep Risen", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d21", "", 42, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 50000m },
                    { 43, "Proline", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d22", "", 43, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 85500m },
                    { 44, "Zero1", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d23", "", 44, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 34200m },
                    { 45, "Welness", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d24", "", 45, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 25000m },
                    { 46, "İzmir Avukat Hareketi (İZAH)", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d25", "", 46, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 370000m },
                    { 47, "Proline (Katalog)", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "d26", "", 47, 1, 5, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 57000m },
                    { 48, "C3 Teknoloji", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p1", "", 48, 1, 4, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 49, "Bilimsev Koleji", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p2", "", 49, 1, 3, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 50, "Sanat Garajı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p3", "", 50, 1, 2, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 51, "İzmir Avukat Hareketi", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p4", "", 51, 1, 4, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 52, "Lazarus", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p5", "", 52, 1, 3, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 53, "Oha Yatch", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p6", "", 53, 1, 4, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 54, "Boatfinder", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p7", "", 54, 1, 4, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 55, "İzmir Cerrahi Onkoloji", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p8", "", 55, 1, 3, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 56, "Wellnes", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p9", "", 56, 1, 3, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 57, "SANGROW", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p10", "", 57, 1, 1, 1, 2, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 58, "Netforce", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p11", "", 58, 1, 1, 1, 2, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 59, "Metod Koleji", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p12", "", 59, 1, 1, 1, 2, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 60, "Proline", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p13", "", 60, 1, 2, 1, 2, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 61, "İlkim Makina", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p14", "", 61, 1, 2, 1, 2, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 62, "4R Mühendislik", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p15", "", 62, 1, 2, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 63, "Arkas Holding", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p16", "", 63, 1, 3, 2, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 64, "Cour de Lion", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "p17", "", 64, 1, 2, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 65, "The Vets Hub", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x1", "", 65, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 20000m },
                    { 66, "Zero1", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x2", "", 66, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 51300m },
                    { 67, "Pekari", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x3", "", 67, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 28500m },
                    { 68, "Funce Medical", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x4", "", 68, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 45000m },
                    { 69, "Baskın Kocabaş", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x5", "", 69, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 35000m },
                    { 70, "Tolga Bıçakcı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x6", "", 70, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 35000m },
                    { 71, "Çiler Ezgi", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x7", "", 71, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 57000m },
                    { 72, "Bjorn Coffe", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x8", "", 72, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 34200m },
                    { 73, "Kaan Akacun", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x9", "", 73, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 33000m },
                    { 74, "Dino Pizza", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x10", "", 74, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 30000m },
                    { 75, "Saygın Health", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x11", "", 75, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 35000m },
                    { 76, "Ecem Cantürk Nazlı", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x12", "", 76, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 51300m },
                    { 77, "Zişan Cin", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x13", "", 77, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 34200m },
                    { 78, "Hakkı Kurt", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x14", "", 78, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 40000m },
                    { 79, "Bilet Points", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x15", "", 79, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 80000m },
                    { 80, "Venettia Pizza", "", new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "x16", "", 80, 1, 6, 1, 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 25000m }
                });

            migrationBuilder.InsertData(
                table: "MarketingLeadOwners",
                columns: new[] { "MarketingLeadId", "MarketingOwnerId", "SortOrder" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 1, 3, 2 },
                    { 2, 1, 1 },
                    { 2, 2, 2 },
                    { 3, 2, 2 },
                    { 3, 3, 1 },
                    { 4, 2, 1 },
                    { 5, 3, 1 },
                    { 6, 4, 1 },
                    { 7, 3, 1 },
                    { 8, 1, 1 },
                    { 9, 2, 1 },
                    { 10, 4, 1 },
                    { 11, 1, 1 },
                    { 12, 3, 1 },
                    { 13, 2, 1 },
                    { 14, 2, 1 },
                    { 15, 2, 1 },
                    { 16, 3, 1 },
                    { 17, 4, 1 },
                    { 18, 3, 1 },
                    { 19, 3, 1 },
                    { 20, 4, 1 },
                    { 21, 1, 1 },
                    { 22, 2, 1 },
                    { 23, 1, 1 },
                    { 23, 2, 2 },
                    { 24, 4, 1 },
                    { 25, 2, 1 },
                    { 26, 1, 1 },
                    { 27, 2, 1 },
                    { 28, 2, 1 },
                    { 29, 3, 1 },
                    { 30, 2, 1 },
                    { 31, 1, 1 },
                    { 32, 2, 1 },
                    { 33, 3, 1 },
                    { 34, 4, 1 },
                    { 35, 2, 1 },
                    { 36, 4, 1 },
                    { 37, 2, 1 },
                    { 38, 2, 1 },
                    { 39, 2, 1 },
                    { 40, 1, 1 },
                    { 41, 3, 1 },
                    { 42, 2, 1 },
                    { 43, 4, 1 },
                    { 44, 4, 1 },
                    { 45, 4, 1 },
                    { 46, 1, 1 },
                    { 46, 2, 2 },
                    { 47, 4, 1 },
                    { 48, 1, 1 },
                    { 49, 1, 1 },
                    { 50, 2, 1 },
                    { 51, 1, 1 },
                    { 52, 3, 1 },
                    { 53, 3, 1 },
                    { 54, 3, 1 },
                    { 55, 4, 1 },
                    { 56, 4, 1 },
                    { 57, 1, 1 },
                    { 58, 1, 1 },
                    { 59, 1, 1 },
                    { 60, 4, 1 },
                    { 61, 4, 1 },
                    { 62, 1, 1 },
                    { 63, 1, 1 },
                    { 64, 3, 1 },
                    { 65, 3, 1 },
                    { 66, 4, 1 },
                    { 67, 2, 1 },
                    { 68, 3, 1 },
                    { 69, 3, 1 },
                    { 70, 3, 1 },
                    { 71, 2, 1 },
                    { 72, 2, 1 },
                    { 73, 3, 1 },
                    { 74, 3, 1 },
                    { 75, 3, 1 },
                    { 76, 4, 1 },
                    { 77, 2, 1 },
                    { 78, 4, 1 },
                    { 79, 4, 1 },
                    { 80, 1, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeadOwners_MarketingOwnerId",
                table: "MarketingLeadOwners",
                column: "MarketingOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeads_ExternalId",
                table: "MarketingLeads",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeads_SourceId",
                table: "MarketingLeads",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeads_StageId",
                table: "MarketingLeads",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeads_StatusId",
                table: "MarketingLeads",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeads_TemperatureId",
                table: "MarketingLeads",
                column: "TemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeadStatuses_Key",
                table: "MarketingLeadStatuses",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingLeadTemperatures_Key",
                table: "MarketingLeadTemperatures",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingOwners_Name",
                table: "MarketingOwners",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingSources_Name",
                table: "MarketingSources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingStages_Key",
                table: "MarketingStages",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketingLeadOwners");

            migrationBuilder.DropTable(
                name: "MarketingLeads");

            migrationBuilder.DropTable(
                name: "MarketingOwners");

            migrationBuilder.DropTable(
                name: "MarketingLeadStatuses");

            migrationBuilder.DropTable(
                name: "MarketingLeadTemperatures");

            migrationBuilder.DropTable(
                name: "MarketingSources");

            migrationBuilder.DropTable(
                name: "MarketingStages");
        }
    }
}
