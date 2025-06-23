using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wirin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentDeliveryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "OrderDeliveries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDeliveryId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDeliveries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDeliveries");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "OrderDeliveries");
        }
    }
}
