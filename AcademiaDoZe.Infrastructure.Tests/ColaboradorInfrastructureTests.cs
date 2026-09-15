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

    public ColaboradorInfrastructureTests()
    {
        _logradouro = new(ConnectionString, DatabaseType);
        _colaborador = new(ConnectionString, DatabaseType);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() { await _logradouro.DisposeAsync(); await _colaborador.DisposeAsync(); }

    private async Task<Colaborador> CriarAsync()
    {
        var logradouro = await _logradouro.Adicionar(Logradouro.Criar(0, GerarCep(), "Kaio", "Fernandes Branco", "SQLite", "SC", "Brasil").Value!);
        var colaborador = Colaborador.Criar(0, "Kaio", GerarCpf(), new DateOnly(1990, 1, 1), "48999999999", $"kaio{Guid.NewGuid():N}@teste.com", logradouro, "1", "Fernandes Branco", "SenhaSQLite123", Arquivo.Criar([1]).Value!, new DateOnly(2020, 1, 1), ColaboradorTipo.Instrutor, ColaboradorVinculo.CLT).Value!;
        return await _colaborador.Adicionar(colaborador);
    }

    private async Task LimparAsync(Colaborador colaborador)
    {
        await _colaborador.Remover(colaborador.Id);
        await _logradouro.Remover(colaborador.Endereco.Logradouro.Id);
    }

    [Fact] public async Task Adicionar_persiste_colaborador() { var c = await CriarAsync(); try { Assert.True(c.Id > 0); } finally { await LimparAsync(c); } }
    [Fact] public async Task Obter_por_id_retorna_colaborador() { var c = await CriarAsync(); try { Assert.Equal("Kaio", (await _colaborador.ObterPorId(c.Id))!.Nome); } finally { await LimparAsync(c); } }
    [Fact] public async Task Obter_todos_retorna_colaboradores() { var c = await CriarAsync(); try { Assert.Contains(await _colaborador.ObterTodos(), item => item.Id == c.Id); } finally { await LimparAsync(c); } }
    [Fact] public async Task Obter_por_cpf_retorna_colaborador() { var c = await CriarAsync(); try { Assert.Equal(c.Id, (await _colaborador.ObterPorCpf(c.Cpf))!.Id); } finally { await LimparAsync(c); } }
    [Fact] public async Task Obter_por_email_retorna_colaborador() { var c = await CriarAsync(); try { Assert.Equal(c.Id, (await _colaborador.ObterPorEmail(c.Email))!.Id); } finally { await LimparAsync(c); } }

    [Fact]
    public async Task Cpf_ja_existe_respeita_o_proprio_id()
    {
        var c = await CriarAsync();
        try { Assert.True(await _colaborador.CpfJaExiste(c.Cpf)); Assert.False(await _colaborador.CpfJaExiste(c.Cpf, c.Id)); }
        finally { await LimparAsync(c); }
    }

    [Fact] public async Task Email_ja_existe_respeita_o_proprio_id() { var c = await CriarAsync(); try { Assert.False(await _colaborador.EmailJaExiste(c.Email, c.Id)); } finally { await LimparAsync(c); } }
    [Fact] public async Task Obter_por_tipo_retorna_colaborador() { var c = await CriarAsync(); try { Assert.Contains(await _colaborador.ObterPorTipo(c.Tipo), item => item.Id == c.Id); } finally { await LimparAsync(c); } }
    [Fact] public async Task Obter_por_vinculo_retorna_colaborador() { var c = await CriarAsync(); try { Assert.Contains(await _colaborador.ObterPorVinculo(c.Vinculo), item => item.Id == c.Id); } finally { await LimparAsync(c); } }
    [Fact] public async Task Trocar_senha_persiste_alteracao() { var c = await CriarAsync(); try { Assert.True(await _colaborador.TrocarSenha(c.Id, Senha.Criar("NovaSenhaSQLite123").Value!)); } finally { await LimparAsync(c); } }

    [Fact]
    public async Task Atualizar_persiste_alteracao()
    {
        var c = await CriarAsync();
        try
        {
            var atualizado = Colaborador.Criar(c.Id, "Kaio", c.Cpf.Valor, c.DataNascimento, c.Telefone.Valor, c.Email.Valor, c.Endereco.Logradouro, "2", "Fernandes Branco", "NovaSenhaSQLite123", c.Foto, c.DataAdmissao, ColaboradorTipo.Administrador, ColaboradorVinculo.CLT).Value!;
            await _colaborador.Atualizar(atualizado);
            Assert.Equal(ColaboradorTipo.Administrador, (await _colaborador.ObterPorId(c.Id))!.Tipo);
        }
        finally { await LimparAsync(c); }
    }

    [Fact]
    public async Task Remover_exclui_colaborador_e_inexistente_retorna_falso()
    {
        var c = await CriarAsync();
        Assert.True(await _colaborador.Remover(c.Id));
        Assert.False(await _colaborador.Remover(999999));
        await _logradouro.Remover(c.Endereco.Logradouro.Id);
    }
}
