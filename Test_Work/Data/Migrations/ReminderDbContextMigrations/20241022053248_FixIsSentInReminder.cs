using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Work.Data.Migrations.ReminderDbContextMigrations
{
    /// <inheritdoc />
    public partial class FixIsSentInReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "Reminders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_ReminderDate_IsSent",
                table: "Reminders",
                columns: new[] { "ReminderDate", "IsSent" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reminders_ReminderDate_IsSent",
                table: "Reminders");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "Reminders",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);
        }
    }
}
