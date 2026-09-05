// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public sealed class MatriculaInfrastructureTests : TestBase, IAsyncLifetime
{
    private readonly LogradouroRepository _logradouroRepository;
    private readonly AlunoRepository _alunoRepository;
    private readonly MatriculaRepository _matriculaRepository;

    public MatriculaInfrastructureTests()
    {
        _logradouroRepository = new LogradouroRepository(ConnectionString, DatabaseType);
        _alunoRepository = new AlunoRepository(ConnectionString, DatabaseType);
        _matriculaRepository = new MatriculaRepository(ConnectionString, DatabaseType);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync()
    {
        await _logradouroRepository.DisposeAsync();
        await _alunoRepository.DisposeAsync();
        await _matriculaRepository.DisposeAsync();
    }

    private async Task<Aluno> CriarAlunoAsync()
    {
        var logradouro = await _logradouroRepository.Adicionar(Logradouro.Criar(0, GerarCep(), "Kaio", "Fernandes Branco", "SQLite", "SC", "Brasil").Value!);
        var foto = Arquivo.Criar([1, 2, 3]).Value!;
        var aluno = Aluno.Criar(0, "Aluno de Teste", GerarCpf(), DateOnly.FromDateTime(DateTime.Today.AddYears(-20)), "48999999999", $"aluno{Guid.NewGuid():N}@teste.com", logradouro, "123", string.Empty, "Senha123", foto).Value!;
        return await _alunoRepository.Adicionar(aluno);
    }

    private async Task<Matricula> CriarMatriculaAsync(Aluno aluno, MatriculaPlano plano = MatriculaPlano.Mensal, DateOnly? inicio = null, MatriculaRestricoes restricoes = MatriculaRestricoes.None)
    {
        var laudo = restricoes == MatriculaRestricoes.None ? null : Arquivo.Criar([10, 20, 30]).Value;
        var matricula = Matricula.Criar(0, aluno, plano, inicio ?? DateOnly.FromDateTime(DateTime.Today), laudo, restricoes, "Kaio", "SQLite").Value!;
        return await _matriculaRepository.Adicionar(matricula);
    }

    [Fact]
    public async Task Adicionar_e_obter_por_id_persistem_todos_os_campos()
    {
        var aluno = await CriarAlunoAsync();
        var flags = MatriculaRestricoes.Diabetes | MatriculaRestricoes.PressaoAlta;
        var matricula = await CriarMatriculaAsync(aluno, MatriculaPlano.Trimestral, restricoes: flags);
        var obtida = await _matriculaRepository.ObterPorId(matricula.Id);
        Assert.NotNull(obtida); Assert.Equal(aluno.Id, obtida.AlunoMatricula.Id); Assert.Equal("Kaio", obtida.Objetivo); Assert.Equal("SQLite", obtida.ObservacoesRestricoes); Assert.Equal(flags, obtida.RestricoesMedicas);
    }

    [Fact]
    public async Task Obter_por_id_inexistente_retorna_nulo() => Assert.Null(await _matriculaRepository.ObterPorId(999999));

    [Fact]
    public async Task Obter_todos_e_por_aluno_funcionam()
    {
        var aluno = await CriarAlunoAsync(); await CriarMatriculaAsync(aluno);
        Assert.NotEmpty(await _matriculaRepository.ObterTodos());
        Assert.All(await _matriculaRepository.ObterPorAluno(aluno.Id), item => Assert.Equal(aluno.Id, item.AlunoMatricula.Id));
    }

    [Fact]
    public async Task Atualizar_persiste_e_inexistente_lanca_excecao()
    {
        var aluno = await CriarAlunoAsync(); var matricula = await CriarMatriculaAsync(aluno);
        var atualizada = Matricula.Criar(matricula.Id, aluno, MatriculaPlano.Anual, matricula.DataInicio, Arquivo.Criar([9]).Value, MatriculaRestricoes.Alergias, "Kaio", "SQLite").Value!;
        Assert.Equal(MatriculaPlano.Anual, (await _matriculaRepository.Atualizar(atualizada)).Plano);
        var inexistente = Matricula.Criar(999999, aluno, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), null, MatriculaRestricoes.None, "Kaio", "SQLite").Value!;
        Assert.Equal("REGISTRO_NAO_ENCONTRADO", (await Assert.ThrowsAsync<InfrastructureException>(() => _matriculaRepository.Atualizar(inexistente))).ErrorCode);
    }

    [Fact]
    public async Task Remover_funciona_e_inexistente_retorna_false()
    {
        var matricula = await CriarMatriculaAsync(await CriarAlunoAsync());
        Assert.True(await _matriculaRepository.Remover(matricula.Id)); Assert.Null(await _matriculaRepository.ObterPorId(matricula.Id)); Assert.False(await _matriculaRepository.Remover(999999));
    }

    [Fact]
    public async Task Consultas_de_matricula_ativa_funcionam()
    {
        var aluno = await CriarAlunoAsync(); Assert.False(await _matriculaRepository.PossuiMatriculaAtiva(aluno.Id));
        await CriarMatriculaAsync(aluno, MatriculaPlano.Mensal);
        Assert.True(await _matriculaRepository.PossuiMatriculaAtiva(aluno.Id)); Assert.NotNull(await _matriculaRepository.ObterMatriculaAtivaPorAluno(aluno.Id));
        Assert.NotEmpty(await _matriculaRepository.ObterAtivas()); Assert.NotEmpty(await _matriculaRepository.ObterAtivas(aluno.Id));
    }

    [Fact]
    public async Task Consultas_por_vencimento_e_plano_funcionam()
    {
        var aluno = await CriarAlunoAsync();
        await CriarMatriculaAsync(aluno, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today.AddDays(-25)));
        Assert.Contains(await _matriculaRepository.ObterVencendoEmDias(30), item => item.AlunoMatricula.Id == aluno.Id);
        var trimestral = await CriarMatriculaAsync(aluno, MatriculaPlano.Trimestral);
        Assert.Contains(await _matriculaRepository.ObterPorPlano(MatriculaPlano.Trimestral), item => item.Id == trimestral.Id);
    }
}
