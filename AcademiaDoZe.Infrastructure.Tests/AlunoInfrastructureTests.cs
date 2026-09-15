// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public sealed class AlunoInfrastructureTests : TestBase, IAsyncLifetime
{
    private readonly LogradouroRepository _logradouro;
    private readonly AlunoRepository _aluno;

    public AlunoInfrastructureTests()
    {
        _logradouro = new(ConnectionString, DatabaseType);
        _aluno = new(ConnectionString, DatabaseType);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() { await _logradouro.DisposeAsync(); await _aluno.DisposeAsync(); }

    private async Task<Aluno> CriarAsync()
    {
        var logradouro = await _logradouro.Adicionar(Logradouro.Criar(0, GerarCep(), "Kaio", "Fernandes Branco", "SQLite", "SC", "Brasil").Value!);
        var aluno = Aluno.Criar(0, "Kaio", GerarCpf(), new DateOnly(2000, 1, 1), "48999999999", $"kaio{Guid.NewGuid():N}@teste.com", logradouro, "1", "Fernandes Branco", "SenhaSQLite123", Arquivo.Criar([1]).Value!).Value!;
        return await _aluno.Adicionar(aluno);
    }

    private async Task LimparAsync(Aluno aluno)
    {
        await _aluno.Remover(aluno.Id);
        await _logradouro.Remover(aluno.Endereco.Logradouro.Id);
    }

    [Fact] public async Task Adicionar_persiste_aluno() { var a = await CriarAsync(); try { Assert.True(a.Id > 0); } finally { await LimparAsync(a); } }
    [Fact] public async Task Obter_por_id_retorna_aluno() { var a = await CriarAsync(); try { Assert.Equal("Kaio", (await _aluno.ObterPorId(a.Id))!.Nome); } finally { await LimparAsync(a); } }
    [Fact] public async Task Obter_todos_retorna_alunos() { var a = await CriarAsync(); try { Assert.Contains(await _aluno.ObterTodos(), item => item.Id == a.Id); } finally { await LimparAsync(a); } }
    [Fact] public async Task Obter_por_cpf_retorna_aluno() { var a = await CriarAsync(); try { Assert.Equal(a.Id, (await _aluno.ObterPorCpf(a.Cpf))!.Id); } finally { await LimparAsync(a); } }
    [Fact] public async Task Obter_por_email_retorna_aluno() { var a = await CriarAsync(); try { Assert.Equal(a.Id, (await _aluno.ObterPorEmail(a.Email))!.Id); } finally { await LimparAsync(a); } }

    [Fact]
    public async Task Cpf_ja_existe_respeita_o_proprio_id()
    {
        var a = await CriarAsync();
        try { Assert.True(await _aluno.CpfJaExiste(a.Cpf)); Assert.False(await _aluno.CpfJaExiste(a.Cpf, a.Id)); }
        finally { await LimparAsync(a); }
    }

    [Fact] public async Task Email_ja_existe_retorna_verdadeiro() { var a = await CriarAsync(); try { Assert.True(await _aluno.EmailJaExiste(a.Email)); } finally { await LimparAsync(a); } }
    [Fact] public async Task Trocar_senha_persiste_alteracao() { var a = await CriarAsync(); try { Assert.True(await _aluno.TrocarSenha(a.Id, Senha.Criar("NovaSenhaSQLite123").Value!)); } finally { await LimparAsync(a); } }

    [Fact]
    public async Task Atualizar_persiste_alteracao()
    {
        var a = await CriarAsync();
        try
        {
            var atualizado = Aluno.Criar(a.Id, "Kaio", a.Cpf.Valor, a.DataNascimento, a.Telefone.Valor, a.Email.Valor, a.Endereco.Logradouro, "2", "Fernandes Branco", "NovaSenhaSQLite123", a.Foto).Value!;
            await _aluno.Atualizar(atualizado);
            Assert.Equal("2", (await _aluno.ObterPorId(a.Id))!.Endereco.Numero);
        }
        finally { await LimparAsync(a); }
    }

    [Fact]
    public async Task Remover_exclui_aluno_e_inexistente_retorna_falso()
    {
        var a = await CriarAsync();
        Assert.True(await _aluno.Remover(a.Id));
        Assert.False(await _aluno.Remover(999999));
        await _logradouro.Remover(a.Endereco.Logradouro.Id);
    }
}
