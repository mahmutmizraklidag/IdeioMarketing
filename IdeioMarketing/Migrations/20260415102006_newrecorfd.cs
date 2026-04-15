using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeioMarketing.Migrations
{
    /// <inheritdoc />
    public partial class newrecorfd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractDurationMonths",
                table: "OfferRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractDurationMonths",
                table: "OfferRecords");
        }
    }
}
