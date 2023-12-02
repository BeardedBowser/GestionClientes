using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionCliente.Migrations
{
    /// <inheritdoc />
    public partial class nuevacolumnalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Resultado",
                table: "Log",
                type: "VARCHAR(200)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resultado",
                table: "Log");
        }
    }
}
