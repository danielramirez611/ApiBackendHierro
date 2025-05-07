using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaWebApiHierro.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContactoAsociadoAPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contactos_Users_UserId",
                table: "Contactos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pacientes_Contactos_RepresentanteId",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_RepresentanteId",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "RepresentanteId",
                table: "Pacientes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Contactos",
                newName: "PacienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Contactos_UserId",
                table: "Contactos",
                newName: "IX_Contactos_PacienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contactos_Pacientes_PacienteId",
                table: "Contactos",
                column: "PacienteId",
                principalTable: "Pacientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contactos_Pacientes_PacienteId",
                table: "Contactos");

            migrationBuilder.RenameColumn(
                name: "PacienteId",
                table: "Contactos",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Contactos_PacienteId",
                table: "Contactos",
                newName: "IX_Contactos_UserId");

            migrationBuilder.AddColumn<int>(
                name: "RepresentanteId",
                table: "Pacientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_RepresentanteId",
                table: "Pacientes",
                column: "RepresentanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contactos_Users_UserId",
                table: "Contactos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pacientes_Contactos_RepresentanteId",
                table: "Pacientes",
                column: "RepresentanteId",
                principalTable: "Contactos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
