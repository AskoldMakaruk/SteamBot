using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
	public partial class fifth : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				"SearchName",
				"Skins",
				"text",
				nullable: true);

			migrationBuilder.CreateIndex(
				"IX_Skins_SkinName_WeaponName",
				"Skins",
				new[] {"SkinName", "WeaponName"},
				unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				"IX_Skins_SkinName_WeaponName",
				"Skins");

			migrationBuilder.DropColumn(
				"SearchName",
				"Skins");
		}
	}
}