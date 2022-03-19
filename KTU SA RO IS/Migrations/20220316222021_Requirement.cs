using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTU_SA_RO.Migrations
{
    public partial class Requirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequirementId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Requirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Is_general = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Is_fulfilled = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Events_RequirementId",
                table: "Events",
                column: "RequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Requirements_RequirementId",
                table: "Events",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Requirements_RequirementId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Events_RequirementId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "Events");
        }
    }
}
