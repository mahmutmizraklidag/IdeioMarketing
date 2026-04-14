using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeioMarketing.Migrations
{
    /// <inheritdoc />
    public partial class categort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPackageMultiSelected",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubPackageMultiSelected",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPackageMultiSelected",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsSubPackageMultiSelected",
                table: "Categories");
        }
    }
}
