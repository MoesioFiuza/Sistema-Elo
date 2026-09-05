namespace Elo.Application.Common.Interfaces;

public interface IEmailSender
{
    Task EnviarAsync(string destinatario, string assunto, string corpoTexto, CancellationToken ct = default);
}
