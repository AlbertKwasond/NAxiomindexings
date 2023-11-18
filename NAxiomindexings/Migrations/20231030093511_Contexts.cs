using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAxiomindexings.Migrations
{
    /// <inheritdoc />
    public partial class Contexts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardOfDirectorsContexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    FullName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: true),
                    Position = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardOfDirectorsContexts", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HomeContexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Messages = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeContexts", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeamContexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: true),
                    Position = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamContexts", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardOfDirectorsContexts");

            migrationBuilder.DropTable(
                name: "HomeContexts");

            migrationBuilder.DropTable(
                name: "TeamContexts");
        }
    }
}
