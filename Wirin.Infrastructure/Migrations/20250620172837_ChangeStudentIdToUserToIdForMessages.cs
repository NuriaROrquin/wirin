using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wirin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStudentIdToUserToIdForMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "studentId",
                table: "Message",
                newName: "userToId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userToId",
                table: "Message",
                newName: "studentId");
        }
    }
}
