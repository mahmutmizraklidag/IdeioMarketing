using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeioMarketing.Migrations
{
    /// <inheritdoc />
    public partial class offerrecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfferRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxOffice = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NotificationAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentPlanId = table.Column<int>(type: "int", nullable: true),
                    PaymentPlanName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PaymentPlanInstallmentCount = table.Column<int>(type: "int", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferRecordInstallments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferRecordId = table.Column<int>(type: "int", nullable: false),
                    MonthNo = table.Column<int>(type: "int", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferRecordInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferRecordInstallments_OfferRecords_OfferRecordId",
                        column: x => x.OfferRecordId,
                        principalTable: "OfferRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferRecordItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferRecordId = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SourceItemId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    RawName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsOneTime = table.Column<bool>(type: "bit", nullable: false),
                    IsPiece = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferRecordItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferRecordItems_OfferRecords_OfferRecordId",
                        column: x => x.OfferRecordId,
                        principalTable: "OfferRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferRecordInstallments_OfferRecordId",
                table: "OfferRecordInstallments",
                column: "OfferRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferRecordItems_OfferRecordId",
                table: "OfferRecordItems",
                column: "OfferRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferRecordInstallments");

            migrationBuilder.DropTable(
                name: "OfferRecordItems");

            migrationBuilder.DropTable(
                name: "OfferRecords");
        }
    }
}
