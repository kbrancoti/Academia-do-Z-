// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
namespace AcademiaDoZe.Domain.Repositories;
public interface IAlunoRepository
{
    Task<Aluno?> ObterPorId(int id, CancellationToken ct = default);
    Task<IEnumerable<Aluno>> ObterTodos(CancellationToken ct = default);
    Task<Aluno> Adicionar(Aluno entity, CancellationToken ct = default);
    Task<Aluno> Atualizar(Aluno entity, CancellationToken ct = default);
    Task<bool> Remover(int id, CancellationToken ct = default);
    Task<Aluno?> ObterPorCpf(Cpf cpf, CancellationToken ct = default);
    Task<Aluno?> ObterPorEmail(Email email, CancellationToken ct = default);
    Task<bool> CpfJaExiste(Cpf cpf, int? id = null, CancellationToken ct = default);
    Task<bool> EmailJaExiste(Email email, int? id = null, CancellationToken ct = default);
    Task<bool> TrocarSenha(int id, Senha senha, CancellationToken ct = default);
}
