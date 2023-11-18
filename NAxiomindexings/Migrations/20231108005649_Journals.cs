using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAxiomindexings.Migrations
{
    /// <inheritdoc />
    public partial class Journals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    MiddleName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Gender = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Contact = table.Column<string>(type: "longtext", nullable: false),
                    Address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    RegistrationStatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DateOfRegistration = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Journals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    AuthorId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TotalPages = table.Column<int>(type: "int", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "longtext", nullable: false),
                    BookPdfUrl = table.Column<string>(type: "longtext", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Journals_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Journals_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    URL = table.Column<string>(type: "longtext", nullable: true),
                    JournalsId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Galleries_Journals_JournalsId",
                        column: x => x.JournalsId,
                        principalTable: "Journals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_JournalsId",
                table: "Galleries",
                column: "JournalsId");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_AuthorId",
                table: "Journals",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_CategoryId",
                table: "Journals",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "Journals");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
