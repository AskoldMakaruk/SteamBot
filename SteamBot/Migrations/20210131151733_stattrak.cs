using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
	public partial class stattrak : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Skins_Images_BattleScarredImageId",
				"Skins");

			migrationBuilder.DropForeignKey(
				"FK_Skins_Images_FactoryNewImageId",
				"Skins");

			migrationBuilder.DropForeignKey(
				"FK_Skins_Images_FieldTestedImageId",
				"Skins");

			migrationBuilder.DropForeignKey(
				"FK_Skins_Images_ImageId",
				"Skins");

			migrationBuilder.DropForeignKey(
				"FK_Skins_Images_MinimalWearImageId",
				"Skins");

			migrationBuilder.DropForeignKey(
				"FK_Skins_Images_WellWornImageId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_Skins_BattleScarredImageId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_Skins_FactoryNewImageId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_Skins_FieldTestedImageId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_Skins_ImageId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_Skins_MinimalWearImageId",
				"Skins");

			migrationBuilder.DropIndex(
				"IX_Skins_WellWornImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"BattleScarredImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"BattleScarredPrice",
				"Skins");

			migrationBuilder.DropColumn(
				"FactoryNewImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"FactoryNewPrice",
				"Skins");

			migrationBuilder.DropColumn(
				"FieldTestedImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"FieldTestedPrice",
				"Skins");

			migrationBuilder.DropColumn(
				"ImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"MinimalWearImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"MinimalWearPrice",
				"Skins");

			migrationBuilder.DropColumn(
				"Price",
				"Skins");

			migrationBuilder.DropColumn(
				"WellWornImageId",
				"Skins");

			migrationBuilder.DropColumn(
				"WellWornPrice",
				"Skins");

			migrationBuilder.CreateTable(
				"SkinPrice",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					StatTrak = table.Column<bool>("boolean", nullable: true),
					Float = table.Column<float>("real", nullable: true),
					FloatName = table.Column<string>("text", nullable: true),
					Value = table.Column<double>("double precision", nullable: false),
					ImageId = table.Column<int>("integer", nullable: true),
					SkinId = table.Column<int>("integer", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SkinPrice", x => x.Id);
					table.ForeignKey(
						"FK_SkinPrice_Images_ImageId",
						x => x.ImageId,
						"Images",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_SkinPrice_Skins_SkinId",
						x => x.SkinId,
						"Skins",
						"Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				"IX_SkinPrice_ImageId",
				"SkinPrice",
				"ImageId");

			migrationBuilder.CreateIndex(
				"IX_SkinPrice_SkinId",
				"SkinPrice",
				"SkinId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"SkinPrice");

			migrationBuilder.AddColumn<int>(
				"BattleScarredImageId",
				"Skins",
				"integer",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				"BattleScarredPrice",
				"Skins",
				"double precision",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				"FactoryNewImageId",
				"Skins",
				"integer",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				"FactoryNewPrice",
				"Skins",
				"double precision",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				"FieldTestedImageId",
				"Skins",
				"integer",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				"FieldTestedPrice",
				"Skins",
				"double precision",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				"ImageId",
				"Skins",
				"integer",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				"MinimalWearImageId",
				"Skins",
				"integer",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				"MinimalWearPrice",
				"Skins",
				"double precision",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				"Price",
				"Skins",
				"double precision",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				"WellWornImageId",
				"Skins",
				"integer",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				"WellWornPrice",
				"Skins",
				"double precision",
				nullable: true);

			migrationBuilder.CreateIndex(
				"IX_Skins_BattleScarredImageId",
				"Skins",
				"BattleScarredImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_FactoryNewImageId",
				"Skins",
				"FactoryNewImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_FieldTestedImageId",
				"Skins",
				"FieldTestedImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_ImageId",
				"Skins",
				"ImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_MinimalWearImageId",
				"Skins",
				"MinimalWearImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_WellWornImageId",
				"Skins",
				"WellWornImageId");

			migrationBuilder.AddForeignKey(
				"FK_Skins_Images_BattleScarredImageId",
				"Skins",
				"BattleScarredImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Skins_Images_FactoryNewImageId",
				"Skins",
				"FactoryNewImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Skins_Images_FieldTestedImageId",
				"Skins",
				"FieldTestedImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Skins_Images_ImageId",
				"Skins",
				"ImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Skins_Images_MinimalWearImageId",
				"Skins",
				"MinimalWearImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Skins_Images_WellWornImageId",
				"Skins",
				"WellWornImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}