// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
namespace AcademiaDoZe.Application.Services;
public sealed class LogradouroService(Func<ILogradouroRepository> repositorio) : ILogradouroService
{
    private ILogradouroRepository Repo => repositorio();
    public async Task<LogradouroDto?> ObterPorIdAsync(int id, CancellationToken ct = default) => (await Repo.ObterPorId(id, ct))?.ToDto();
    public async Task<IEnumerable<LogradouroDto>> ObterTodosAsync(CancellationToken ct = default) => (await Repo.ObterTodos(ct)).Select(x => x.ToDto());
    public async Task<LogradouroDto> AdicionarAsync(LogradouroDto dto, CancellationToken ct = default) { ArgumentNullException.ThrowIfNull(dto); var cep = Cep.Criar(dto.Cep).Value ?? throw new ArgumentException("CEP inválido."); if (await Repo.CepJaExiste(cep, null, ct)) throw new InvalidOperationException("CEP já cadastrado."); return (await Repo.Adicionar(dto.ToEntity(), ct)).ToDto(); }
    public async Task<LogradouroDto> AtualizarAsync(LogradouroDto dto, CancellationToken ct = default) { ArgumentNullException.ThrowIfNull(dto); if (await Repo.ObterPorId(dto.Id, ct) is null) throw new KeyNotFoundException("Logradouro não encontrado."); return (await Repo.Atualizar(dto.ToEntity(), ct)).ToDto(); }
    public Task<bool> RemoverAsync(int id, CancellationToken ct = default) => Repo.Remover(id, ct);
    public async Task<LogradouroDto?> ObterPorCepAsync(string cep, CancellationToken ct = default) { var value = Cep.Criar(cep).Value ?? throw new ArgumentException("CEP inválido."); return (await Repo.ObterPorCep(value, ct))?.ToDto(); }
    public async Task<bool> CepJaExisteAsync(string cep, int? id = null, CancellationToken ct = default) { var value = Cep.Criar(cep).Value ?? throw new ArgumentException("CEP inválido."); return await Repo.CepJaExiste(value, id, ct); }
    public async Task<IEnumerable<LogradouroDto>> ObterPorCidadeAsync(string cidade, CancellationToken ct = default) => (await Repo.ObterPorCidade(cidade, ct)).Select(x => x.ToDto());
    public async Task<IEnumerable<LogradouroDto>> ObterPorBairroAsync(string cidade, string bairro, CancellationToken ct = default) => (await Repo.ObterPorBairro(cidade, bairro, ct)).Select(x => x.ToDto());
}
