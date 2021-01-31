using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
    public partial class stattrak : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skins_Images_BattleScarredImageId",
                table: "Skins");

            migrationBuilder.DropForeignKey(
                name: "FK_Skins_Images_FactoryNewImageId",
                table: "Skins");

            migrationBuilder.DropForeignKey(
                name: "FK_Skins_Images_FieldTestedImageId",
                table: "Skins");

            migrationBuilder.DropForeignKey(
                name: "FK_Skins_Images_ImageId",
                table: "Skins");

            migrationBuilder.DropForeignKey(
                name: "FK_Skins_Images_MinimalWearImageId",
                table: "Skins");

            migrationBuilder.DropForeignKey(
                name: "FK_Skins_Images_WellWornImageId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_Skins_BattleScarredImageId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_Skins_FactoryNewImageId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_Skins_FieldTestedImageId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_Skins_ImageId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_Skins_MinimalWearImageId",
                table: "Skins");

            migrationBuilder.DropIndex(
                name: "IX_Skins_WellWornImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "BattleScarredImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "BattleScarredPrice",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "FactoryNewImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "FactoryNewPrice",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "FieldTestedImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "FieldTestedPrice",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "MinimalWearImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "MinimalWearPrice",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "WellWornImageId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "WellWornPrice",
                table: "Skins");

            migrationBuilder.CreateTable(
                name: "SkinPrice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatTrak = table.Column<bool>(type: "boolean", nullable: true),
                    Float = table.Column<float>(type: "real", nullable: true),
                    FloatName = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    ImageId = table.Column<int>(type: "integer", nullable: true),
                    SkinId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkinPrice_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkinPrice_Skins_SkinId",
                        column: x => x.SkinId,
                        principalTable: "Skins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkinPrice_ImageId",
                table: "SkinPrice",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_SkinPrice_SkinId",
                table: "SkinPrice",
                column: "SkinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkinPrice");

            migrationBuilder.AddColumn<int>(
                name: "BattleScarredImageId",
                table: "Skins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BattleScarredPrice",
                table: "Skins",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FactoryNewImageId",
                table: "Skins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FactoryNewPrice",
                table: "Skins",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FieldTestedImageId",
                table: "Skins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FieldTestedPrice",
                table: "Skins",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Skins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimalWearImageId",
                table: "Skins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinimalWearPrice",
                table: "Skins",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Skins",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WellWornImageId",
                table: "Skins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WellWornPrice",
                table: "Skins",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skins_BattleScarredImageId",
                table: "Skins",
                column: "BattleScarredImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_FactoryNewImageId",
                table: "Skins",
                column: "FactoryNewImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_FieldTestedImageId",
                table: "Skins",
                column: "FieldTestedImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_ImageId",
                table: "Skins",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_MinimalWearImageId",
                table: "Skins",
                column: "MinimalWearImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_WellWornImageId",
                table: "Skins",
                column: "WellWornImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_Images_BattleScarredImageId",
                table: "Skins",
                column: "BattleScarredImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_Images_FactoryNewImageId",
                table: "Skins",
                column: "FactoryNewImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_Images_FieldTestedImageId",
                table: "Skins",
                column: "FieldTestedImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_Images_ImageId",
                table: "Skins",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_Images_MinimalWearImageId",
                table: "Skins",
                column: "MinimalWearImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_Images_WellWornImageId",
                table: "Skins",
                column: "WellWornImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
