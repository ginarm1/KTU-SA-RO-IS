using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTU_SA_RO.Migrations
{
    public partial class EventTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventTeamId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventTeams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTeams_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EventTeamId",
                table: "AspNetUsers",
                column: "EventTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTeams_EventId",
                table: "EventTeams",
                column: "EventId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EventTeams_EventTeamId",
                table: "AspNetUsers",
                column: "EventTeamId",
                principalTable: "EventTeams",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EventTeams_EventTeamId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EventTeams");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EventTeamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EventTeamId",
                table: "AspNetUsers");
        }
    }
}
