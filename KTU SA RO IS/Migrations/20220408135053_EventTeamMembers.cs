using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTU_SA_RO.Migrations
{
    public partial class EventTeamMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "EventTeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Is_event_coord = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTeamMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventTeamMembers_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EventTeamMembers_EventId",
                table: "EventTeamMembers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTeamMembers_UserId",
                table: "EventTeamMembers",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTeamMembers");

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
                    Is_event_coord = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
    }
}
