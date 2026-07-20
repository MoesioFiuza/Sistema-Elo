using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacoesIsolamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataLiberacaoIsolamento",
                table: "resultados_laboratoriais",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LiberacaoIsolamentoEnviada",
                table: "resultados_laboratoriais",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsolamentoAtivo",
                table: "internacoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "notificacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioDestinoId = table.Column<Guid>(type: "uuid", nullable: true),
                    PerfilDestino = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Tipo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SolicitacaoExameId = table.Column<Guid>(type: "uuid", nullable: true),
                    Lida = table.Column<bool>(type: "boolean", nullable: false),
                    LidaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notificacoes_solicitacoes_exame_SolicitacaoExameId",
                        column: x => x.SolicitacaoExameId,
                        principalTable: "solicitacoes_exame",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_notificacoes_usuarios_UsuarioDestinoId",
                        column: x => x.UsuarioDestinoId,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_CriadoEm",
                table: "notificacoes",
                column: "CriadoEm");

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_Lida_PerfilDestino",
                table: "notificacoes",
                columns: new[] { "Lida", "PerfilDestino" });

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_SolicitacaoExameId",
                table: "notificacoes",
                column: "SolicitacaoExameId");

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_UsuarioDestinoId",
                table: "notificacoes",
                column: "UsuarioDestinoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notificacoes");

            migrationBuilder.DropColumn(
                name: "DataLiberacaoIsolamento",
                table: "resultados_laboratoriais");

            migrationBuilder.DropColumn(
                name: "LiberacaoIsolamentoEnviada",
                table: "resultados_laboratoriais");

            migrationBuilder.DropColumn(
                name: "IsolamentoAtivo",
                table: "internacoes");
        }
    }
}
