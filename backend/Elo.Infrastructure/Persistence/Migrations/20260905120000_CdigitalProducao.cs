using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CdigitalProducao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Setor",
                table: "usuarios",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QualidadeAmostra",
                table: "solicitacoes_exame",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "NaoAvaliada");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAvaliacaoAmostra",
                table: "solicitacoes_exame",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssinaturaBase64",
                table: "resultados_laboratoriais",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssinadoPorNome",
                table: "resultados_laboratoriais",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssinadoEm",
                table: "resultados_laboratoriais",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaudoAnexoNome",
                table: "resultados_laboratoriais",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaudoAnexoContentType",
                table: "resultados_laboratoriais",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "LaudoAnexoBytes",
                table: "resultados_laboratoriais",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LaudoGeradoEm",
                table: "resultados_laboratoriais",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "solicitacoes_acesso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PerfilSolicitado = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Setor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Justificativa = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MotivoRecusa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RevisadoPorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RevisadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioCriadoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitacoes_acesso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_solicitacoes_acesso_usuarios_RevisadoPorId",
                        column: x => x.RevisadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_solicitacoes_acesso_usuarios_UsuarioCriadoId",
                        column: x => x.UsuarioCriadoId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_acesso_Email",
                table: "solicitacoes_acesso",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_acesso_Status",
                table: "solicitacoes_acesso",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_acesso_RevisadoPorId",
                table: "solicitacoes_acesso",
                column: "RevisadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_acesso_UsuarioCriadoId",
                table: "solicitacoes_acesso",
                column: "UsuarioCriadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "solicitacoes_acesso");

            migrationBuilder.DropColumn(name: "Setor", table: "usuarios");
            migrationBuilder.DropColumn(name: "QualidadeAmostra", table: "solicitacoes_exame");
            migrationBuilder.DropColumn(name: "DataAvaliacaoAmostra", table: "solicitacoes_exame");
            migrationBuilder.DropColumn(name: "AssinaturaBase64", table: "resultados_laboratoriais");
            migrationBuilder.DropColumn(name: "AssinadoPorNome", table: "resultados_laboratoriais");
            migrationBuilder.DropColumn(name: "AssinadoEm", table: "resultados_laboratoriais");
            migrationBuilder.DropColumn(name: "LaudoAnexoNome", table: "resultados_laboratoriais");
            migrationBuilder.DropColumn(name: "LaudoAnexoContentType", table: "resultados_laboratoriais");
            migrationBuilder.DropColumn(name: "LaudoAnexoBytes", table: "resultados_laboratoriais");
            migrationBuilder.DropColumn(name: "LaudoGeradoEm", table: "resultados_laboratoriais");
        }
    }
}
