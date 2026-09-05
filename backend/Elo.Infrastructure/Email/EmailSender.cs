using System.Net;
using System.Net.Mail;
using Elo.Application.Common.Interfaces;
using Elo.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elo.Infrastructure.Email;

public class EmailSender(IOptions<SmtpOptions> options, ILogger<EmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions _smtp = options.Value;

    public async Task EnviarAsync(string destinatario, string assunto, string corpoTexto, CancellationToken ct = default)
    {
        if (!_smtp.Enabled || string.IsNullOrWhiteSpace(_smtp.Host))
        {
            logger.LogInformation("SMTP desligado. E-mail para {Destinatario}: {Assunto}\n{Corpo}", destinatario, assunto, corpoTexto);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(string.IsNullOrWhiteSpace(_smtp.From) ? _smtp.User : _smtp.From),
            Subject = assunto,
            Body = corpoTexto,
            IsBodyHtml = false,
        };
        message.To.Add(destinatario);

        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            EnableSsl = _smtp.UseStartTls,
        };

        if (!string.IsNullOrWhiteSpace(_smtp.User))
            client.Credentials = new NetworkCredential(_smtp.User, _smtp.Password);

        await client.SendMailAsync(message, ct);
        logger.LogInformation("E-mail enviado para {Destinatario}: {Assunto}", destinatario, assunto);
    }
}
