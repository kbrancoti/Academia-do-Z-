// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
namespace AcademiaDoZe.Domain.Repositories;
public interface IColaboradorRepository
{
    Task<Colaborador?> ObterPorId(int id, CancellationToken ct = default);
    Task<IEnumerable<Colaborador>> ObterTodos(CancellationToken ct = default);
    Task<Colaborador> Adicionar(Colaborador entity, CancellationToken ct = default);
    Task<Colaborador> Atualizar(Colaborador entity, CancellationToken ct = default);
    Task<bool> Remover(int id, CancellationToken ct = default);
    Task<Colaborador?> ObterPorCpf(Cpf cpf, CancellationToken ct = default);
    Task<Colaborador?> ObterPorEmail(Email email, CancellationToken ct = default);
    Task<bool> CpfJaExiste(Cpf cpf, int? id = null, CancellationToken ct = default);
    Task<bool> EmailJaExiste(Email email, int? id = null, CancellationToken ct = default);
    Task<IEnumerable<Colaborador>> ObterPorTipo(ColaboradorTipo tipo, CancellationToken ct = default);
    Task<IEnumerable<Colaborador>> ObterPorVinculo(ColaboradorVinculo vinculo, CancellationToken ct = default);
    Task<bool> TrocarSenha(int id, Senha senha, CancellationToken ct = default);
}
