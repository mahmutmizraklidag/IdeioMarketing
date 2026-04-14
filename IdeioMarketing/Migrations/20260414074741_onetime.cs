using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeioMarketing.Migrations
{
    /// <inheritdoc />
    public partial class onetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOneTime",
                table: "SubPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOneTime",
                table: "Packages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOneTime",
                table: "SubPackages");

            migrationBuilder.DropColumn(
                name: "IsOneTime",
                table: "Packages");
        }
    }
}
