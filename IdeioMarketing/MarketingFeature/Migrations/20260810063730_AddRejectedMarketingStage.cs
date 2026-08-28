using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeioMarketing.MarketingFeature.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectedMarketingStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MarketingStages",
                columns: new[] { "Id", "Color", "Key", "Label", "SortOrder" },
                values: new object[] { 7, "#C65F7B", "rejected", "Reddedildi", 7 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MarketingStages",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
