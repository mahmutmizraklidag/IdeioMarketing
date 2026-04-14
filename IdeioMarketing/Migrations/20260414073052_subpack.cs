using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeioMarketing.Migrations
{
    /// <inheritdoc />
    public partial class subpack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPiece",
                table: "SubPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPiece",
                table: "SubPackages");
        }
    }
}
