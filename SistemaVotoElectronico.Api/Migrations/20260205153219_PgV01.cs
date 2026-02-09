using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaVotoElectronico.Api.Migrations
{
    /// <inheritdoc />
    public partial class PgV01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventosElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosElectorales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreRol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolUsuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListasPoliticas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Eslogan = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: false),
                    EventoElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasPoliticas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasPoliticas_EventosElectorales_EventoElectoralId",
                        column: x => x.EventoElectoralId,
                        principalTable: "EventosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Clave = table.Column<string>(type: "text", nullable: false),
                    YaVoto = table.Column<bool>(type: "boolean", nullable: false),
                    TokenVotacion = table.Column<string>(type: "text", nullable: true),
                    RolUsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_RolUsuarios_RolUsuarioId",
                        column: x => x.RolUsuarioId,
                        principalTable: "RolUsuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    FotoUrl = table.Column<string>(type: "text", nullable: false),
                    PlanGobiernoUrl = table.Column<string>(type: "text", nullable: false),
                    ListaPoliticaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatos_ListasPoliticas_ListaPoliticaId",
                        column: x => x.ListaPoliticaId,
                        principalTable: "ListasPoliticas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultadosElecciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalVotos = table.Column<int>(type: "integer", nullable: false),
                    EventoElectoralId = table.Column<int>(type: "integer", nullable: false),
                    ListaPoliticaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosElecciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadosElecciones_EventosElectorales_EventoElectoralId",
                        column: x => x.EventoElectoralId,
                        principalTable: "EventosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultadosElecciones_ListasPoliticas_ListaPoliticaId",
                        column: x => x.ListaPoliticaId,
                        principalTable: "ListasPoliticas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HashSeguridad = table.Column<string>(type: "text", nullable: false),
                    EventoElectoralId = table.Column<int>(type: "integer", nullable: false),
                    ListaPoliticaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_EventosElectorales_EventoElectoralId",
                        column: x => x.EventoElectoralId,
                        principalTable: "EventosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votos_ListasPoliticas_ListaPoliticaId",
                        column: x => x.ListaPoliticaId,
                        principalTable: "ListasPoliticas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Certificados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoQR = table.Column<string>(type: "text", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EventoElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificados_EventosElectorales_EventoElectoralId",
                        column: x => x.EventoElectoralId,
                        principalTable: "EventosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_ListaPoliticaId",
                table: "Candidatos",
                column: "ListaPoliticaId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_EventoElectoralId",
                table: "Certificados",
                column: "EventoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_UsuarioId",
                table: "Certificados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasPoliticas_EventoElectoralId",
                table: "ListasPoliticas",
                column: "EventoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosElecciones_EventoElectoralId",
                table: "ResultadosElecciones",
                column: "EventoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosElecciones_ListaPoliticaId",
                table: "ResultadosElecciones",
                column: "ListaPoliticaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cedula",
                table: "Usuarios",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolUsuarioId",
                table: "Usuarios",
                column: "RolUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TokenVotacion",
                table: "Usuarios",
                column: "TokenVotacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votos_EventoElectoralId",
                table: "Votos",
                column: "EventoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ListaPoliticaId",
                table: "Votos",
                column: "ListaPoliticaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "Certificados");

            migrationBuilder.DropTable(
                name: "ResultadosElecciones");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "ListasPoliticas");

            migrationBuilder.DropTable(
                name: "RolUsuarios");

            migrationBuilder.DropTable(
                name: "EventosElectorales");
        }
    }
}
