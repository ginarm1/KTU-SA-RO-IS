using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTU_SA_RO.Migrations
{
    public partial class RequirementEventId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Requirements_RequirementId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_RequirementId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Requirements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_EventId",
                table: "Requirements",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Events_EventId",
                table: "Requirements",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Events_EventId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_EventId",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Requirements");

            migrationBuilder.AddColumn<int>(
                name: "RequirementId",
                table: "Events",
                type: "int",
                nullable: true);

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
    }
}
