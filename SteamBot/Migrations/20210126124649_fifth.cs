using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
    public partial class fifth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchName",
                table: "Skins",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skins_SkinName_WeaponName",
                table: "Skins",
                columns: new[] { "SkinName", "WeaponName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skins_SkinName_WeaponName",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "SearchName",
                table: "Skins");
        }
    }
}
