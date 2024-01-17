using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAxiomindexings.Migrations
{
    /// <inheritdoc />
    public partial class Updateblog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageTitle",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UrlHandle",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<bool>(
                name: "Visible",
                table: "BlogPosts",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Heading",
                table: "BlogPosts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Visible",
                table: "BlogPosts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Heading",
                table: "BlogPosts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "PageTitle",
                table: "BlogPosts",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "BlogPosts",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "UrlHandle",
                table: "BlogPosts",
                type: "longtext",
                nullable: false);
        }
    }
}
