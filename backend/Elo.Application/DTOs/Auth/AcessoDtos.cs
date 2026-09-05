using Elo.Domain.Enums;

namespace Elo.Application.DTOs.Auth;

public record SolicitarAcessoRequest(
    string Nome,
    string Email,
    PerfilUsuario PerfilSolicitado,
    string? Setor = null,
    string? Justificativa = null);

public record SolicitarAcessoResponse(string Mensagem, Guid Id);

public record SolicitacaoAcessoDto(
    Guid Id,
    string Nome,
    string Email,
    PerfilUsuario PerfilSolicitado,
    string? Setor,
    string? Justificativa,
    StatusSolicitacaoAcesso Status,
    string? MotivoRecusa,
    DateTime CriadoEm,
    DateTime? RevisadoEm);

public record AprovarAcessoRequest(string? SenhaInicial = null);

public record RecusarAcessoRequest(string Motivo);

public record AprovarAcessoResponse(
    Guid UsuarioId,
    string Email,
    string Nome,
    PerfilUsuario Perfil,
    string SenhaInicial);
