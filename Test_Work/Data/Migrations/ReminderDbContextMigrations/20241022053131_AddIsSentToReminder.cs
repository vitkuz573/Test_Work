using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Work.Data.Migrations.ReminderDbContextMigrations
{
    /// <inheritdoc />
    public partial class AddIsSentToReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "Reminders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "Reminders");
        }
    }
}
