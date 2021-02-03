using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
	public partial class eight : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Skins_SteamItem_SteamItemId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_SteamItem_Id",
				"SteamItem");

			migrationBuilder.DropIndex(
				"IX_Skins_SteamItemId",
				"Skins");

			migrationBuilder.AddColumn<int>(
				"SkinId",
				"SteamItem",
				"integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateIndex(
				"IX_SteamItem_SkinId",
				"SteamItem",
				"SkinId",
				unique: true);

			migrationBuilder.AddForeignKey(
				"FK_SteamItem_Skins_SkinId",
				"SteamItem",
				"SkinId",
				"Skins",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_SteamItem_Skins_SkinId",
				"SteamItem");

			migrationBuilder.DropIndex(
				"IX_SteamItem_SkinId",
				"SteamItem");

			migrationBuilder.DropColumn(
				"SkinId",
				"SteamItem");

			migrationBuilder.CreateIndex(
				"IX_SteamItem_Id",
				"SteamItem",
				"Id");

			migrationBuilder.CreateIndex(
				"IX_Skins_SteamItemId",
				"Skins",
				"SteamItemId",
				unique: true);

			migrationBuilder.AddForeignKey(
				"FK_Skins_SteamItem_SteamItemId",
				"Skins",
				"SteamItemId",
				"SteamItem",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}