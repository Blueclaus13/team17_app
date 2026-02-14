using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulMomentsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Mood",
                table: "Entries",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "JournalId",
                table: "Entries",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Activity",
                table: "Entries",
                type: "text",
                nullable: false,
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Mood",
                table: "Entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "JournalId",
                table: "Entries",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Activity",
                table: "Entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
