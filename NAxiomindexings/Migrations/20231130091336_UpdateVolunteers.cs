using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAxiomindexings.Migrations
{
	/// <inheritdoc />
	public partial class UpdateVolunteers : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
		 name: "ImageUrl",
		 table: "Volunteers",
		 type: "longtext",
		 nullable: false,
		 oldClrType: typeof(string),
		 oldType: "longtext",
		 oldNullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
	name: "ImageUrl",
	table: "Volunteers",
	defaultValue: "",
	oldClrType: typeof(string),
	oldType: "longtext");
		}
	}
}