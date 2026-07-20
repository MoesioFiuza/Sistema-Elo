using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroProntuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    Sexo = table.Column<int>(type: "integer", nullable: false),
                    HistoricoDiarreiaPrevia = table.Column<int>(type: "integer", nullable: false),
                    HistoricoCdiff = table.Column<int>(type: "integer", nullable: false),
                    HistoricoCovid = table.Column<int>(type: "integer", nullable: false),
                    HistoricoTransplante = table.Column<int>(type: "integer", nullable: false),
                    HistoricoQuimioterapia = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Perfil = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "internacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Enfermaria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Leito = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DataInternacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MotivoInternacao = table.Column<string>(type: "text", nullable: true),
                    EmUti = table.Column<int>(type: "integer", nullable: false),
                    Leucocitose = table.Column<int>(type: "integer", nullable: false),
                    Sepse = table.Column<int>(type: "integer", nullable: false),
                    Obito = table.Column<int>(type: "integer", nullable: false),
                    DataObito = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_internacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_internacoes_pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auditoria_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Acao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DadosAnteriores = table.Column<string>(type: "text", nullable: true),
                    DadosNovos = table.Column<string>(type: "text", nullable: true),
                    EnderecoIp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_auditoria_logs_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "solicitacoes_exame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    InternacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CarimboDataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdAmostraUnico = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DataColeta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataRecebimentoLaboratorio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitacoes_exame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_solicitacoes_exame_internacoes_InternacaoId",
                        column: x => x.InternacaoId,
                        principalTable: "internacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitacoes_exame_pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitacoes_exame_usuarios_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "formularios_clinicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitacaoExameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Diarreia = table.Column<int>(type: "integer", nullable: false),
                    EpisodiosDiarreia24h = table.Column<int>(type: "integer", nullable: true),
                    ConsistenciaFezes = table.Column<int>(type: "integer", nullable: false),
                    DorAbdominal = table.Column<int>(type: "integer", nullable: false),
                    Febre = table.Column<int>(type: "integer", nullable: false),
                    TemperaturaMaxima = table.Column<decimal>(type: "numeric", nullable: true),
                    Peritonite = table.Column<int>(type: "integer", nullable: false),
                    IleoParalitico = table.Column<int>(type: "integer", nullable: false),
                    Megacolon = table.Column<int>(type: "integer", nullable: false),
                    UsoIbp = table.Column<int>(type: "integer", nullable: false),
                    IbpDescricao = table.Column<string>(type: "text", nullable: true),
                    UsoAntimicrobiano30d = table.Column<int>(type: "integer", nullable: false),
                    AntimicrobianosDescricao = table.Column<string>(type: "text", nullable: true),
                    VentilacaoMecanica = table.Column<int>(type: "integer", nullable: false),
                    NutricaoParenteral = table.Column<int>(type: "integer", nullable: false),
                    SondaNasogastrica = table.Column<int>(type: "integer", nullable: false),
                    ObservacoesClinicas = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formularios_clinicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_formularios_clinicos_solicitacoes_exame_SolicitacaoExameId",
                        column: x => x.SolicitacaoExameId,
                        principalTable: "solicitacoes_exame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resultados_laboratoriais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitacaoExameId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponsavelId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataResultado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TesteRapido = table.Column<int>(type: "integer", nullable: false),
                    ToxinaA = table.Column<int>(type: "integer", nullable: false),
                    ToxinaB = table.Column<int>(type: "integer", nullable: false),
                    Cultura = table.Column<int>(type: "integer", nullable: false),
                    CepaIdentificada = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ObservacoesLaboratorio = table.Column<string>(type: "text", nullable: true),
                    AlertaPositivoEnviado = table.Column<bool>(type: "boolean", nullable: false),
                    DataAlertaEnviado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultados_laboratoriais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resultados_laboratoriais_solicitacoes_exame_SolicitacaoExam~",
                        column: x => x.SolicitacaoExameId,
                        principalTable: "solicitacoes_exame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resultados_laboratoriais_usuarios_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tratamentos_cdiff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitacaoExameId = table.Column<Guid>(type: "uuid", nullable: false),
                    IniciouTratamento = table.Column<int>(type: "integer", nullable: false),
                    DataInicioTratamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Medicacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Dose = table.Column<string>(type: "text", nullable: true),
                    DuracaoDias = table.Column<int>(type: "integer", nullable: true),
                    RespostaDia7 = table.Column<int>(type: "integer", nullable: false),
                    RespostaFinal = table.Column<int>(type: "integer", nullable: false),
                    Recidiva = table.Column<int>(type: "integer", nullable: false),
                    DataRecidiva = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ObservacoesTratamento = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tratamentos_cdiff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tratamentos_cdiff_solicitacoes_exame_SolicitacaoExameId",
                        column: x => x.SolicitacaoExameId,
                        principalTable: "solicitacoes_exame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_logs_DataHora",
                table: "auditoria_logs",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_logs_Entidade_EntidadeId",
                table: "auditoria_logs",
                columns: new[] { "Entidade", "EntidadeId" });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_logs_UsuarioId",
                table: "auditoria_logs",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_formularios_clinicos_SolicitacaoExameId",
                table: "formularios_clinicos",
                column: "SolicitacaoExameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_internacoes_PacienteId",
                table: "internacoes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_NumeroProntuario",
                table: "pacientes",
                column: "NumeroProntuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resultados_laboratoriais_ResponsavelId",
                table: "resultados_laboratoriais",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_resultados_laboratoriais_SolicitacaoExameId",
                table: "resultados_laboratoriais",
                column: "SolicitacaoExameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_exame_IdAmostraUnico",
                table: "solicitacoes_exame",
                column: "IdAmostraUnico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_exame_InternacaoId",
                table: "solicitacoes_exame",
                column: "InternacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_exame_PacienteId",
                table: "solicitacoes_exame",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_exame_SolicitanteId",
                table: "solicitacoes_exame",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_tratamentos_cdiff_SolicitacaoExameId",
                table: "tratamentos_cdiff",
                column: "SolicitacaoExameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria_logs");

            migrationBuilder.DropTable(
                name: "formularios_clinicos");

            migrationBuilder.DropTable(
                name: "resultados_laboratoriais");

            migrationBuilder.DropTable(
                name: "tratamentos_cdiff");

            migrationBuilder.DropTable(
                name: "solicitacoes_exame");

            migrationBuilder.DropTable(
                name: "internacoes");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "pacientes");
        }
    }
}
