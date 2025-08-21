using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageLengthConstraint512 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Messages",
                type: "TEXT",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Messages",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 512);
        }
    }
}
