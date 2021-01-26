using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
    public partial class eight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skins_SteamItem_SteamItemId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_SteamItem_Id",
                table: "SteamItem");

            migrationBuilder.DropIndex(
                name: "IX_Skins_SteamItemId",
                table: "Skins");

            migrationBuilder.AddColumn<int>(
                name: "SkinId",
                table: "SteamItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SteamItem_SkinId",
                table: "SteamItem",
                column: "SkinId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SteamItem_Skins_SkinId",
                table: "SteamItem",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SteamItem_Skins_SkinId",
                table: "SteamItem");

            migrationBuilder.DropIndex(
                name: "IX_SteamItem_SkinId",
                table: "SteamItem");

            migrationBuilder.DropColumn(
                name: "SkinId",
                table: "SteamItem");

            migrationBuilder.CreateIndex(
                name: "IX_SteamItem_Id",
                table: "SteamItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_SteamItemId",
                table: "Skins",
                column: "SteamItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_SteamItem_SteamItemId",
                table: "Skins",
                column: "SteamItemId",
                principalTable: "SteamItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
