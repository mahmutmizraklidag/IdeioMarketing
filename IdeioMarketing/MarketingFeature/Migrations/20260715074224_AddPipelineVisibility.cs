using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeioMarketing.MarketingFeature.Migrations
{
    /// <inheritdoc />
    public partial class AddPipelineVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInPipeline",
                table: "MarketingLeads",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 12,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 13,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 14,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 15,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 16,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 17,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 18,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 19,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 20,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 21,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 22,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 23,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 24,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 25,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 26,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 27,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 28,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 29,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 30,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 31,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 32,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 33,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 34,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 35,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 36,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 37,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 38,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 39,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 40,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 41,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 42,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 43,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 44,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 45,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 46,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 47,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 48,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 49,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 50,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 51,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 52,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 53,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 54,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 55,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 56,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 57,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 58,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 59,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 60,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 61,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 62,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 63,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 64,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 65,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 66,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 67,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 68,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 69,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 70,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 71,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 72,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 73,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 74,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 75,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 76,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 77,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 78,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 79,
                column: "IsInPipeline",
                value: true);

            migrationBuilder.UpdateData(
                table: "MarketingLeads",
                keyColumn: "Id",
                keyValue: 80,
                column: "IsInPipeline",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInPipeline",
                table: "MarketingLeads");
        }
    }
}
