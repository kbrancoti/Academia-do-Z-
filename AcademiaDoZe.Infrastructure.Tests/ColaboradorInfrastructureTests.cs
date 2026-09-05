// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public sealed class ColaboradorInfrastructureTests : TestBase, IAsyncLifetime
{
    private readonly LogradouroRepository _logradouro;
    private readonly ColaboradorRepository _colaborador;
    public ColaboradorInfrastructureTests() { _logradouro = new(ConnectionString, DatabaseType); _colaborador = new(ConnectionString, DatabaseType); }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() { await _logradouro.DisposeAsync(); await _colaborador.DisposeAsync(); }
    private async Task<Colaborador> CriarAsync()
    {
        var l = await _logradouro.Adicionar(Logradouro.Criar(0, GerarCep(), "Kaio", "Fernandes Branco", "SQLite", "SC", "Brasil").Value!);
        var c = Colaborador.Criar(0, "Kaio", GerarCpf(), new DateOnly(1990, 1, 1), "48999999999", $"kaio{Guid.NewGuid():N}@teste.com", l, "1", "Fernandes Branco", "SenhaSQLite123", Arquivo.Criar([1]).Value!, new DateOnly(2020, 1, 1), ColaboradorTipo.Instrutor, ColaboradorVinculo.CLT).Value!;
        return await _colaborador.Adicionar(c);
    }
    [Fact] public async Task Colaborador_crud_filtros_e_consultas_funcionam()
    {
        var c = await CriarAsync(); Assert.True(c.Id > 0); Assert.Equal("Kaio", (await _colaborador.ObterPorId(c.Id))!.Nome); Assert.NotEmpty(await _colaborador.ObterTodos());
        Assert.Equal(c.Id, (await _colaborador.ObterPorCpf(c.Cpf))!.Id); Assert.Equal(c.Id, (await _colaborador.ObterPorEmail(c.Email))!.Id); Assert.True(await _colaborador.CpfJaExiste(c.Cpf)); Assert.False(await _colaborador.EmailJaExiste(c.Email, c.Id)); Assert.Contains(await _colaborador.ObterPorTipo(c.Tipo), x => x.Id == c.Id); Assert.Contains(await _colaborador.ObterPorVinculo(c.Vinculo), x => x.Id == c.Id);
        Assert.True(await _colaborador.TrocarSenha(c.Id, Senha.Criar("NovaSenhaSQLite123").Value!));
        var atualizado = Colaborador.Criar(c.Id, "Kaio", c.Cpf.Valor, c.DataNascimento, c.Telefone.Valor, c.Email.Valor, c.Endereco.Logradouro, "2", "Fernandes Branco", "NovaSenhaSQLite123", c.Foto, c.DataAdmissao, ColaboradorTipo.Administrador, ColaboradorVinculo.CLT).Value!;
        await _colaborador.Atualizar(atualizado); Assert.Equal(ColaboradorTipo.Administrador, (await _colaborador.ObterPorId(c.Id))!.Tipo); Assert.True(await _colaborador.Remover(c.Id)); Assert.False(await _colaborador.Remover(999999));
    }
}
