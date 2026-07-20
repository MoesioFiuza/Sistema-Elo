using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FormularioEpidemiologicoCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CdiffFamiliaAmbiente",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CovidAnosPositivos",
                table: "pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CovidDiasInternacao",
                table: "pacientes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CovidDiasIntubado",
                table: "pacientes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CovidInternado",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CovidIntubado",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CovidOxigenioOuTratamentos",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CovidQuandoIntubacao",
                table: "pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CovidSintomasDescricao",
                table: "pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CovidTeveSintomas",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CovidTratamentosDescricao",
                table: "pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CovidUtiDuranteIntubacao",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiarreiaAssociadaAtbPassado",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InternadoPorDiarreia",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProblemasSaudeAdjacentes",
                table: "pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProblemasSaudeOutros",
                table: "pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcurouAtendimentoPorDiarreia",
                table: "pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "QuandoInternadoPorDiarreia",
                table: "pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImunossupressoresDescricao",
                table: "internacoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InternouComDiarreia",
                table: "internacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Leucopenia",
                table: "internacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParaTcth",
                table: "internacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParaTos",
                table: "internacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoCirurgia",
                table: "internacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsoImunossupressoresAtual",
                table: "internacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsoImunossupressoresDurante",
                table: "internacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AntimicrobianosAntesDescricao",
                table: "formularios_clinicos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AntimicrobianosDiaColetaDescricao",
                table: "formularios_clinicos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesorientacaoConfusao",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiasInicioSintomas",
                table: "formularios_clinicos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DrogasVasoativas",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DuracaoFebre",
                table: "formularios_clinicos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FezIra",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InternouUtiDurante",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Leucocitose",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Leucopenia",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SintomasAssociados",
                table: "formularios_clinicos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsoAntimicrobianoAntesColeta",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsoAntimicrobianoDiaColeta",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsoIbpAntesDiarreia",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsoIbpDuranteDiarreia",
                table: "formularios_clinicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CdiffFamiliaAmbiente",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidAnosPositivos",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidDiasInternacao",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidDiasIntubado",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidInternado",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidIntubado",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidOxigenioOuTratamentos",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidQuandoIntubacao",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidSintomasDescricao",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidTeveSintomas",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidTratamentosDescricao",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "CovidUtiDuranteIntubacao",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "DiarreiaAssociadaAtbPassado",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "InternadoPorDiarreia",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "ProblemasSaudeAdjacentes",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "ProblemasSaudeOutros",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "ProcurouAtendimentoPorDiarreia",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "QuandoInternadoPorDiarreia",
                table: "pacientes");

            migrationBuilder.DropColumn(
                name: "ImunossupressoresDescricao",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "InternouComDiarreia",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "Leucopenia",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "ParaTcth",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "ParaTos",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "TipoCirurgia",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "UsoImunossupressoresAtual",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "UsoImunossupressoresDurante",
                table: "internacoes");

            migrationBuilder.DropColumn(
                name: "AntimicrobianosAntesDescricao",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "AntimicrobianosDiaColetaDescricao",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "DesorientacaoConfusao",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "DiasInicioSintomas",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "DrogasVasoativas",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "DuracaoFebre",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "FezIra",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "InternouUtiDurante",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "Leucocitose",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "Leucopenia",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "SintomasAssociados",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "UsoAntimicrobianoAntesColeta",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "UsoAntimicrobianoDiaColeta",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "UsoIbpAntesDiarreia",
                table: "formularios_clinicos");

            migrationBuilder.DropColumn(
                name: "UsoIbpDuranteDiarreia",
                table: "formularios_clinicos");
        }
    }
}
