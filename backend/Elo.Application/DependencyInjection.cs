using Elo.Application.Services;
using Elo.Application.Options;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPacienteService, PacienteService>();
        services.AddScoped<ISolicitacaoService, SolicitacaoService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificacaoService, NotificacaoService>();
        services.AddScoped<ITratamentoService, TratamentoService>();
        return services;
    }
}
