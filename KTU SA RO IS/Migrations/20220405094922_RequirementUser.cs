using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTU_SA_RO.Migrations
{
    public partial class RequirementUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Requirements",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_UserId",
                table: "Requirements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_AspNetUsers_UserId",
                table: "Requirements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_AspNetUsers_UserId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_UserId",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Requirements");
        }
    }
}
