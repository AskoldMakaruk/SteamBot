using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
	public partial class nineth : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_SteamItem_Skins_SkinId",
				"SteamItem");

			migrationBuilder.DropPrimaryKey(
				"PK_SteamItem",
				"SteamItem");

			migrationBuilder.RenameTable(
				"SteamItem",
				newName: "SteamItems");

			migrationBuilder.RenameIndex(
				"IX_SteamItem_SkinId",
				table: "SteamItems",
				newName: "IX_SteamItems_SkinId");

			migrationBuilder.AddPrimaryKey(
				"PK_SteamItems",
				"SteamItems",
				"Id");

			migrationBuilder.AddForeignKey(
				"FK_SteamItems_Skins_SkinId",
				"SteamItems",
				"SkinId",
				"Skins",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_SteamItems_Skins_SkinId",
				"SteamItems");

			migrationBuilder.DropPrimaryKey(
				"PK_SteamItems",
				"SteamItems");

			migrationBuilder.RenameTable(
				"SteamItems",
				newName: "SteamItem");

			migrationBuilder.RenameIndex(
				"IX_SteamItems_SkinId",
				table: "SteamItem",
				newName: "IX_SteamItem_SkinId");

			migrationBuilder.AddPrimaryKey(
				"PK_SteamItem",
				"SteamItem",
				"Id");

			migrationBuilder.AddForeignKey(
				"FK_SteamItem_Skins_SkinId",
				"SteamItem",
				"SkinId",
				"Skins",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}