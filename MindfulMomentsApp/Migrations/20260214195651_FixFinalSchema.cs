using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulMomentsApp.Migrations
{
    /// <inheritdoc />
    public partial class FixFinalSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Journals_UserId",
                table: "Journals");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_UserId",
                table: "Journals",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Journals_UserId",
                table: "Journals");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_UserId",
                table: "Journals",
                column: "UserId");
        }
    }
}
