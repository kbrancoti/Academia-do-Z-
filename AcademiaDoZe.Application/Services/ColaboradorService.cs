// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public sealed class ColaboradorService(Func<IColaboradorRepository> colaboradores, Func<ILogradouroRepository> logradouros) : IColaboradorService
{
    private IColaboradorRepository Repo => colaboradores();
    private async Task<ColaboradorDto> SalvarAsync(ColaboradorDto dto, bool atualizar, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var cpf = Cpf.Criar(dto.Cpf).Value ?? throw new ArgumentException("CPF inválido.");
        if (await Repo.CpfJaExiste(cpf, atualizar ? dto.Id : null, ct)) throw new InvalidOperationException("CPF já cadastrado.");
        var email = Email.Criar(dto.Email ?? string.Empty).Value ?? throw new ArgumentException("E-mail inválido.");
        if (await Repo.EmailJaExiste(email, atualizar ? dto.Id : null, ct)) throw new InvalidOperationException("E-mail já cadastrado.");
        var endereco = dto.Endereco is null ? throw new ArgumentException("Endereço obrigatório.") : await logradouros().ObterPorId(dto.Endereco.Id, ct) ?? throw new KeyNotFoundException("Logradouro não encontrado.");
        var existente = atualizar ? await Repo.ObterPorId(dto.Id, ct) ?? throw new KeyNotFoundException("Colaborador não encontrado.") : null;
        var senha = string.IsNullOrWhiteSpace(dto.Senha) ? existente?.Senha.Valor ?? throw new ArgumentException("Senha obrigatória.") : PasswordHasher.Hash(dto.Senha);
        var entity = dto.ToEntity(endereco, senha);
        return (atualizar ? await Repo.Atualizar(entity, ct) : await Repo.Adicionar(entity, ct)).ToDto();
    }
    public async Task<ColaboradorDto?> ObterPorIdAsync(int id, CancellationToken ct = default) => (await Repo.ObterPorId(id, ct))?.ToDto();
    public async Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(CancellationToken ct = default) => (await Repo.ObterTodos(ct)).Select(x => x.ToDto());
    public Task<ColaboradorDto> AdicionarAsync(ColaboradorDto dto, CancellationToken ct = default) => SalvarAsync(dto, false, ct);
    public Task<ColaboradorDto> AtualizarAsync(ColaboradorDto dto, CancellationToken ct = default) => SalvarAsync(dto, true, ct);
    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default) => await Repo.ObterPorId(id, ct) is not null && await Repo.Remover(id, ct);
    public async Task<ColaboradorDto?> ObterPorCpfAsync(string cpf, CancellationToken ct = default) { var value = Cpf.Criar(cpf).Value ?? throw new ArgumentException("CPF inválido."); return (await Repo.ObterPorCpf(value, ct))?.ToDto(); }
    public async Task<ColaboradorDto?> ObterPorEmailAsync(string email, CancellationToken ct = default) { var value = Email.Criar(email).Value ?? throw new ArgumentException("E-mail inválido."); return (await Repo.ObterPorEmail(value, ct))?.ToDto(); }
    public async Task<IEnumerable<ColaboradorDto>> ObterPorTipoAsync(AppColaboradorTipo tipo, CancellationToken ct = default) => (await Repo.ObterPorTipo(tipo.ToDomain(), ct)).Select(x => x.ToDto());
    public async Task<IEnumerable<ColaboradorDto>> ObterPorVinculoAsync(AppColaboradorVinculo vinculo, CancellationToken ct = default) => (await Repo.ObterPorVinculo(vinculo.ToDomain(), ct)).Select(x => x.ToDto());
    public async Task<bool> CpfJaExisteAsync(string cpf, int? id = null, CancellationToken ct = default) { var value = Cpf.Criar(cpf).Value ?? throw new ArgumentException("CPF inválido."); return await Repo.CpfJaExiste(value, id, ct); }
    public async Task<bool> EmailJaExisteAsync(string email, int? id = null, CancellationToken ct = default) { var value = Email.Criar(email).Value ?? throw new ArgumentException("E-mail inválido."); return await Repo.EmailJaExiste(value, id, ct); }
    public async Task<bool> TrocarSenhaAsync(int id, string senha, CancellationToken ct = default) { if (Senha.Criar(senha).IsFailure) throw new ArgumentException("Senha inválida."); return await Repo.TrocarSenha(id, Senha.Criar(PasswordHasher.Hash(senha)).Value!, ct); }
}
