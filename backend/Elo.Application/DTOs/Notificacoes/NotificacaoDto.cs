using Elo.Domain.Enums;

namespace Elo.Application.DTOs.Notificacoes;

public record NotificacaoDto(
    Guid Id,
    TipoNotificacao Tipo,
    string Titulo,
    string Mensagem,
    Guid? SolicitacaoExameId,
    bool Lida,
    DateTime CriadoEm);
