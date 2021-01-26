using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
    public partial class fourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Skins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFloated",
                table: "Skins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Skins",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skins_ImageId",
                table: "Skins",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_Images_ImageId",
                table: "Skins",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skins_Images_ImageId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_Skins_ImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "IsFloated",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Skins");
        }
    }
}
