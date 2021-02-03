using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
	public partial class fourth : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				"ImageId",
				"Skins",
				"integer",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				"IsFloated",
				"Skins",
				"boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<double>(
				"Price",
				"Skins",
				"double precision",
				nullable: true);

			migrationBuilder.CreateIndex(
				"IX_Skins_ImageId",
				"Skins",
				"ImageId");

			migrationBuilder.AddForeignKey(
				"FK_Skins_Images_ImageId",
				"Skins",
				"ImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Skins_Images_ImageId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_Skins_ImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"ImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"IsFloated",
				"Skins");

			migrationBuilder.DropColumn(
				"Price",
				"Skins");
		}
	}
}