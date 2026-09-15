// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public sealed class MatriculaService(Func<IMatriculaRepository> matriculas, Func<IAlunoRepository> alunos) : IMatriculaService
{
    private IMatriculaRepository Repo => matriculas();
    private async Task<MatriculaDto> MapearAsync(AcademiaDoZe.Domain.Entities.Matricula matricula, CancellationToken ct)
    {
        var aluno = await alunos().ObterPorId(matricula.AlunoMatricula.Id, ct) ?? throw new InvalidOperationException("Aluno da matrícula não encontrado.");
        var dto = matricula.ToDto();
        dto.AlunoMatricula = aluno.ToDto();
        return dto;
    }
    public async Task<MatriculaDto?> ObterPorIdAsync(int id, CancellationToken ct = default) { var value = await Repo.ObterPorId(id, ct); return value is null ? null : await MapearAsync(value, ct); }
    public async Task<IEnumerable<MatriculaDto>> ObterTodasAsync(CancellationToken ct = default) => await ConverterAsync(await Repo.ObterTodos(ct), ct);
    public async Task<MatriculaDto> AdicionarAsync(MatriculaDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var aluno = await alunos().ObterPorId(dto.AlunoMatricula.Id, ct) ?? throw new KeyNotFoundException("Aluno não encontrado.");
        if (await Repo.PossuiMatriculaAtiva(aluno.Id, ct)) throw new InvalidOperationException("O aluno já possui matrícula ativa.");
        var entity = dto.ToEntity(aluno);
        return await MapearAsync(await Repo.Adicionar(entity, ct), ct);
    }
    public async Task<MatriculaDto> AtualizarAsync(MatriculaDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var atual = await Repo.ObterPorId(dto.Id, ct) ?? throw new KeyNotFoundException("Matrícula não encontrada.");
        var aluno = await alunos().ObterPorId(atual.AlunoMatricula.Id, ct) ?? throw new KeyNotFoundException("Aluno não encontrado.");
        return await MapearAsync(await Repo.Atualizar(dto.ToEntity(aluno), ct), ct);
    }
    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default) => await Repo.ObterPorId(id, ct) is not null && await Repo.Remover(id, ct);
    public async Task<IEnumerable<MatriculaDto>> ObterPorAlunoIdAsync(int alunoId, CancellationToken ct = default) => await ConverterAsync(await Repo.ObterPorAluno(alunoId, ct), ct);
    public async Task<MatriculaDto?> ObterMatriculaAtivaPorAlunoAsync(int alunoId, CancellationToken ct = default) { var value = await Repo.ObterMatriculaAtivaPorAluno(alunoId, ct); return value is null ? null : await MapearAsync(value, ct); }
    public Task<bool> PossuiMatriculaAtivaAsync(int alunoId, CancellationToken ct = default) => Repo.PossuiMatriculaAtiva(alunoId, ct);
    public async Task<IEnumerable<MatriculaDto>> ObterAtivasAsync(int alunoId = 0, CancellationToken ct = default) => await ConverterAsync(await Repo.ObterAtivas(alunoId, ct), ct);
    public async Task<IEnumerable<MatriculaDto>> ObterVencendoEmDiasAsync(int dias, CancellationToken ct = default) => await ConverterAsync(await Repo.ObterVencendoEmDias(dias, ct), ct);
    public async Task<IEnumerable<MatriculaDto>> ObterPorPlanoAsync(AppMatriculaPlano plano, CancellationToken ct = default) => await ConverterAsync(await Repo.ObterPorPlano(plano.ToDomain(), ct), ct);
    private async Task<IEnumerable<MatriculaDto>> ConverterAsync(IEnumerable<AcademiaDoZe.Domain.Entities.Matricula> values, CancellationToken ct) => await Task.WhenAll(values.Select(x => MapearAsync(x, ct)));
}
