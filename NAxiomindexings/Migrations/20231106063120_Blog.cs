using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAxiomindexings.Migrations
{
    /// <inheritdoc />
    public partial class Blog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HomeContexts",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Messages",
                table: "HomeContexts",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Heading = table.Column<string>(type: "longtext", nullable: false),
                    PageTitle = table.Column<string>(type: "longtext", nullable: false),
                    Context = table.Column<string>(type: "longtext", nullable: false),
                    ShortDescription = table.Column<string>(type: "longtext", nullable: false),
                    FeaturedImageUrl = table.Column<string>(type: "longtext", nullable: false),
                    UrlHandle = table.Column<string>(type: "longtext", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Author = table.Column<string>(type: "longtext", nullable: false),
                    Visible = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    DisplayName = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BlogPostTag",
                columns: table => new
                {
                    TagsId = table.Column<Guid>(type: "char(36)", nullable: false),
                    blogPostsId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTag", x => new { x.TagsId, x.blogPostsId });
                    table.ForeignKey(
                        name: "FK_BlogPostTag_BlogPosts_blogPostsId",
                        column: x => x.blogPostsId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTag_blogPostsId",
                table: "BlogPostTag",
                column: "blogPostsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostTag");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HomeContexts",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Messages",
                table: "HomeContexts",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldMaxLength: 120);
        }
    }
}
