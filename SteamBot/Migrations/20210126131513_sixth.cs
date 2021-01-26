using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
    public partial class sixth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skins_SkinName_WeaponName",
                table: "Skins");

            migrationBuilder.AlterColumn<string>(
                name: "WeaponName",
                table: "Skins",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SkinName",
                table: "Skins",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "IX_Fullname",
                table: "Skins",
                columns: new[] { "SkinName", "WeaponName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "IX_Fullname",
                table: "Skins");

            migrationBuilder.AlterColumn<string>(
                name: "WeaponName",
                table: "Skins",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SkinName",
                table: "Skins",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_SkinName_WeaponName",
                table: "Skins",
                columns: new[] { "SkinName", "WeaponName" },
                unique: true);
        }
    }
}
