using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
    public partial class nineth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SteamItem_Skins_SkinId",
                table: "SteamItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SteamItem",
                table: "SteamItem");

            migrationBuilder.RenameTable(
                name: "SteamItem",
                newName: "SteamItems");

            migrationBuilder.RenameIndex(
                name: "IX_SteamItem_SkinId",
                table: "SteamItems",
                newName: "IX_SteamItems_SkinId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SteamItems",
                table: "SteamItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SteamItems_Skins_SkinId",
                table: "SteamItems",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SteamItems_Skins_SkinId",
                table: "SteamItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SteamItems",
                table: "SteamItems");

            migrationBuilder.RenameTable(
                name: "SteamItems",
                newName: "SteamItem");

            migrationBuilder.RenameIndex(
                name: "IX_SteamItems_SkinId",
                table: "SteamItem",
                newName: "IX_SteamItem_SkinId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SteamItem",
                table: "SteamItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SteamItem_Skins_SkinId",
                table: "SteamItem",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
