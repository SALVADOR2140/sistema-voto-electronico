using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaVotoElectronico.Api.Migrations
{
    /// <inheritdoc />
    public partial class TablaVotantesExcel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Votantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    HaVotado = table.Column<bool>(type: "boolean", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votantes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votantes_Cedula",
                table: "Votantes",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votantes_Token",
                table: "Votantes",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votantes");
        }
    }
}
