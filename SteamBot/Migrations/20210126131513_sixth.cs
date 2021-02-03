using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
	public partial class sixth : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				"IX_Skins_SkinName_WeaponName",
				"Skins");

			migrationBuilder.AlterColumn<string>(
				"WeaponName",
				"Skins",
				"text",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				"SkinName",
				"Skins",
				"text",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AddUniqueConstraint(
				"IX_Fullname",
				"Skins",
				new[] {"SkinName", "WeaponName"});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropUniqueConstraint(
				"IX_Fullname",
				"Skins");

			migrationBuilder.AlterColumn<string>(
				"WeaponName",
				"Skins",
				"text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text");

			migrationBuilder.AlterColumn<string>(
				"SkinName",
				"Skins",
				"text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text");

			migrationBuilder.CreateIndex(
				"IX_Skins_SkinName_WeaponName",
				"Skins",
				new[] {"SkinName", "WeaponName"},
				unique: true);
		}
	}
}