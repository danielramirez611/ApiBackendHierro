using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaWebApiHierro.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContactosV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Contactos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_UserId",
                table: "Contactos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contactos_Users_UserId",
                table: "Contactos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contactos_Users_UserId",
                table: "Contactos");

            migrationBuilder.DropIndex(
                name: "IX_Contactos_UserId",
                table: "Contactos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contactos");
        }
    }
}
