using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulMomentsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Journals_JournalId1",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_JournalId1",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "JournalId1",
                table: "Entries");

            migrationBuilder.AlterColumn<int>(
                name: "JournalId",
                table: "Entries",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "JournalId",
                table: "Entries",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "JournalId1",
                table: "Entries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entries_JournalId1",
                table: "Entries",
                column: "JournalId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Journals_JournalId1",
                table: "Entries",
                column: "JournalId1",
                principalTable: "Journals",
                principalColumn: "JournalId");
        }
    }
}
