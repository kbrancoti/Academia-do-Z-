// Kaio Fernandes Branco
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ILogradouroService, LogradouroService>();
        services.AddTransient<IAlunoService, AlunoService>();
        services.AddTransient<IColaboradorService, ColaboradorService>();
        services.AddTransient<IMatriculaService, MatriculaService>();
        services.AddTransient(provider => (Func<ILogradouroRepository>)(() => { var c = provider.GetRequiredService<RepositoryConfig>(); return new LogradouroRepository(c.ConnectionString, c.DatabaseType); }));
        services.AddTransient(provider => (Func<IAlunoRepository>)(() => { var c = provider.GetRequiredService<RepositoryConfig>(); return new AlunoRepository(c.ConnectionString, c.DatabaseType); }));
        services.AddTransient(provider => (Func<IColaboradorRepository>)(() => { var c = provider.GetRequiredService<RepositoryConfig>(); return new ColaboradorRepository(c.ConnectionString, c.DatabaseType); }));
        services.AddTransient(provider => (Func<IMatriculaRepository>)(() => { var c = provider.GetRequiredService<RepositoryConfig>(); return new MatriculaRepository(c.ConnectionString, c.DatabaseType); }));
        return services;
    }
}
