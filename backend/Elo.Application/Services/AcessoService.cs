using System.Security.Cryptography;
using Elo.Application.Common;
using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Auth;
using Elo.Application.Options;
using Elo.Domain.Entities;
using Elo.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elo.Application.Services;

public class AcessoService(
    IApplicationDbContext db,
    ICurrentUser currentUser,
    IAuditoriaService auditoria,
    IEmailSender emailSender,
    IOptions<PlataformaOptions> plataformaOptions,
    ILogger<AcessoService> logger) : IAcessoService
{
    private readonly PasswordHasher<Usuario> _hasher = new();
    private readonly PlataformaOptions _plataforma = plataformaOptions.Value;

    public async Task<SolicitarAcessoResponse> SolicitarAsync(
        SolicitarAcessoRequest request,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var nome = request.Nome.Trim();

        if (await db.Usuarios.AnyAsync(u => u.Email.ToLower() == email, ct))
            throw new ConflictException("Já existe uma conta com este e-mail. Use a tela de login.");

        if (await db.SolicitacoesAcesso.AnyAsync(
                s => s.Email.ToLower() == email && s.Status == StatusSolicitacaoAcesso.Pendente, ct))
            throw new ConflictException("Já existe um pedido de acesso pendente para este e-mail.");

        if (request.PerfilSolicitado == PerfilUsuario.Admin)
            throw new ValidationAppException("O perfil de administrador não pode ser solicitado.");

        var pedido = new SolicitacaoAcesso
        {
            Nome = nome,
            Email = email,
            PerfilSolicitado = request.PerfilSolicitado,
            Setor = string.IsNullOrWhiteSpace(request.Setor) ? null : request.Setor.Trim(),
            Justificativa = string.IsNullOrWhiteSpace(request.Justificativa)
                ? null
                : request.Justificativa.Trim(),
            Status = StatusSolicitacaoAcesso.Pendente,
        };

        pedido.Id = Guid.NewGuid();
        db.SolicitacoesAcesso.Add(pedido);
        auditoria.Registrar("SolicitacaoAcesso", pedido.Id, "solicitar", email);
        await db.SaveChangesAsync(ct);

        try
        {
            await emailSender.EnviarAsync(
                _plataforma.AdminEmail,
                $"[{_plataforma.Nome}] Novo pedido de acesso",
                $"""
                Novo pedido de acesso à plataforma {_plataforma.Nome}.

                Nome: {nome}
                E-mail: {email}
                Perfil: {request.PerfilSolicitado}
                Setor: {pedido.Setor ?? "—"}
                Justificativa: {pedido.Justificativa ?? "—"}

                Aprove ou recuse no painel administrativo: {_plataforma.FrontendUrl}/admin
                """,
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao notificar admin sobre pedido de acesso {Email}", email);
        }

        return new SolicitarAcessoResponse(
            $"Pedido enviado. A administradora ({_plataforma.AdminEmail}) receberá a solicitação e liberará o acesso individual.",
            pedido.Id);
    }

    public async Task<IReadOnlyList<SolicitacaoAcessoDto>> ListarAsync(CancellationToken ct = default)
    {
        return await db.SolicitacoesAcesso
            .AsNoTracking()
            .OrderByDescending(s => s.CriadoEm)
            .Select(s => new SolicitacaoAcessoDto(
                s.Id,
                s.Nome,
                s.Email,
                s.PerfilSolicitado,
                s.Setor,
                s.Justificativa,
                s.Status,
                s.MotivoRecusa,
                s.CriadoEm,
                s.RevisadoEm))
            .ToListAsync(ct);
    }

    public async Task<AprovarAcessoResponse> AprovarAsync(
        Guid id,
        AprovarAcessoRequest request,
        CancellationToken ct = default)
    {
        var pedido = await db.SolicitacoesAcesso.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Pedido de acesso não encontrado.");

        if (pedido.Status != StatusSolicitacaoAcesso.Pendente)
            throw new ValidationAppException("Este pedido já foi revisado.");

        if (await db.Usuarios.AnyAsync(u => u.Email.ToLower() == pedido.Email.ToLower(), ct))
            throw new ConflictException("Já existe um usuário com este e-mail.");

        var senha = string.IsNullOrWhiteSpace(request.SenhaInicial)
            ? GerarSenhaTemporaria()
            : request.SenhaInicial.Trim();

        if (senha.Length < 8)
            throw new ValidationAppException("A senha inicial deve ter pelo menos 8 caracteres.");

        var usuario = new Usuario
        {
            Nome = pedido.Nome,
            Email = pedido.Email,
            Perfil = pedido.PerfilSolicitado,
            Setor = pedido.Setor,
            Ativo = true,
        };
        usuario.SenhaHash = _hasher.HashPassword(usuario, senha);

        db.Usuarios.Add(usuario);

        pedido.Status = StatusSolicitacaoAcesso.Aprovada;
        pedido.RevisadoPorId = currentUser.UsuarioId;
        pedido.RevisadoEm = DateTime.UtcNow;
        pedido.UsuarioCriado = usuario;

        auditoria.Registrar("SolicitacaoAcesso", pedido.Id, "aprovar", pedido.Email);
        await db.SaveChangesAsync(ct);

        try
        {
            await emailSender.EnviarAsync(
                pedido.Email,
                $"[{_plataforma.Nome}] Acesso aprovado",
                $"""
                Olá, {pedido.Nome}.

                Seu acesso individual à plataforma {_plataforma.Nome} foi aprovado.

                E-mail: {pedido.Email}
                Perfil: {pedido.PerfilSolicitado}
                Senha inicial: {senha}

                Entre em {_plataforma.FrontendUrl}/login e altere a senha com a equipe, se necessário.
                """,
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao enviar senha inicial para {Email}", pedido.Email);
        }

        return new AprovarAcessoResponse(usuario.Id, usuario.Email, usuario.Nome, usuario.Perfil, senha);
    }

    public async Task RecusarAsync(Guid id, RecusarAcessoRequest request, CancellationToken ct = default)
    {
        var pedido = await db.SolicitacoesAcesso.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Pedido de acesso não encontrado.");

        if (pedido.Status != StatusSolicitacaoAcesso.Pendente)
            throw new ValidationAppException("Este pedido já foi revisado.");

        if (string.IsNullOrWhiteSpace(request.Motivo))
            throw new ValidationAppException("Informe o motivo da recusa.");

        pedido.Status = StatusSolicitacaoAcesso.Recusada;
        pedido.MotivoRecusa = request.Motivo.Trim();
        pedido.RevisadoPorId = currentUser.UsuarioId;
        pedido.RevisadoEm = DateTime.UtcNow;

        auditoria.Registrar("SolicitacaoAcesso", pedido.Id, "recusar", pedido.Email);
        await db.SaveChangesAsync(ct);
    }

    private static string GerarSenhaTemporaria()
    {
        const string alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
        var bytes = RandomNumberGenerator.GetBytes(12);
        var chars = bytes.Select(b => alfabeto[b % alfabeto.Length]).ToArray();
        return new string(chars) + "!";
    }
}
