namespace Elo.Application.Services;

public interface IAuditoriaService
{
    void Registrar(string entidade, Guid entidadeId, string acao, string? dadosNovos = null);
}
