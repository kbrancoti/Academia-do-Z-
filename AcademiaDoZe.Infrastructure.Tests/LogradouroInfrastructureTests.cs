// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public sealed class LogradouroInfrastructureTests : TestBase, IAsyncLifetime
{
    private readonly LogradouroRepository _repository;

    public LogradouroInfrastructureTests() => _repository = new LogradouroRepository(ConnectionString, DatabaseType);
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _repository.DisposeAsync();

    private async Task<Logradouro> CriarEInserirAsync(string? cidade = null, string? bairro = null)
    {
        var result = Logradouro.Criar(0, GerarCep(), "Kaio", bairro ?? "Fernandes Branco", cidade ?? "SQLite", "SC", "Brasil");
        Assert.True(result.IsSuccess);
        return await _repository.Adicionar(result.Value!);
    }

    [Fact]
    public async Task Adicionar_e_obter_por_id_funcionam()
    {
        var inserido = await CriarEInserirAsync();
        Assert.True(inserido.Id > 0);
        var obtido = await _repository.ObterPorId(inserido.Id);
        Assert.NotNull(obtido); Assert.Equal("Kaio", obtido.Nome); Assert.Equal("Fernandes Branco", obtido.Bairro); Assert.Equal("SQLite", obtido.Cidade);
    }

    [Fact]
    public async Task Obter_por_id_inexistente_retorna_nulo() => Assert.Null(await _repository.ObterPorId(999999));

    [Fact]
    public async Task Obter_todos_retorna_registros()
    {
        await CriarEInserirAsync();
        Assert.NotEmpty(await _repository.ObterTodos());
    }

    [Fact]
    public async Task Atualizar_persiste_dados()
    {
        var original = await CriarEInserirAsync();
        var atualizado = Logradouro.Criar(original.Id, GerarCep(), "Kaio", "Fernandes Branco", "SQLite", "SC", "Brasil").Value!;
        await _repository.Atualizar(atualizado);
        var banco = await _repository.ObterPorId(original.Id);
        Assert.NotNull(banco); Assert.Equal(atualizado.Cep.Valor, banco.Cep.Valor);
    }

    [Fact]
    public async Task Atualizar_inexistente_lanca_excecao()
    {
        var entity = Logradouro.Criar(999999, GerarCep(), "Kaio", "Fernandes Branco", "SQLite", "SC", "Brasil").Value!;
        var exception = await Assert.ThrowsAsync<InfrastructureException>(() => _repository.Atualizar(entity));
        Assert.Equal("REGISTRO_NAO_ENCONTRADO", exception.ErrorCode);
    }

    [Fact]
    public async Task Remover_retorna_true_e_exclui()
    {
        var entity = await CriarEInserirAsync();
        Assert.True(await _repository.Remover(entity.Id));
        Assert.Null(await _repository.ObterPorId(entity.Id));
    }

    [Fact]
    public async Task Remover_inexistente_retorna_false() => Assert.False(await _repository.Remover(999999));

    [Fact]
    public async Task Obter_por_cep_e_validar_cep_existente_funcionam()
    {
        var entity = await CriarEInserirAsync();
        Assert.Equal(entity.Id, (await _repository.ObterPorCep(entity.Cep))!.Id);
        Assert.True(await _repository.CepJaExiste(entity.Cep));
        Assert.False(await _repository.CepJaExiste(entity.Cep, entity.Id));
    }

    [Fact]
    public async Task Consultas_por_cidade_e_bairro_funcionam()
    {
        var city = "SQLite";
        var neighborhood = "Fernandes Branco";
        await CriarEInserirAsync(city, neighborhood);
        Assert.Contains(await _repository.ObterPorCidade(city), x => x.Cidade == city);
        Assert.Contains(await _repository.ObterPorBairro(city, neighborhood), x => x.Bairro == neighborhood);
        Assert.Empty(await _repository.ObterPorBairro(city, "Inexistente"));
    }
}
