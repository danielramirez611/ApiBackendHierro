using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaWebApiHierro.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoEnumToTambo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HorarioAtencion",
                table: "Tambos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DocumentoRepresentante",
                table: "Tambos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Referencia",
                table: "Tambos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Tambos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentoRepresentante",
                table: "Tambos");

            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "Tambos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Tambos");

            migrationBuilder.AlterColumn<string>(
                name: "HorarioAtencion",
                table: "Tambos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
