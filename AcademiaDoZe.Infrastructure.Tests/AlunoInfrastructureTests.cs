// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public sealed class AlunoInfrastructureTests : TestBase, IAsyncLifetime
{
    private readonly LogradouroRepository _logradouro;
    private readonly AlunoRepository _aluno;
    public AlunoInfrastructureTests() { _logradouro = new(ConnectionString, DatabaseType); _aluno = new(ConnectionString, DatabaseType); }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() { await _logradouro.DisposeAsync(); await _aluno.DisposeAsync(); }
    private async Task<Aluno> CriarAsync()
    {
        var l = await _logradouro.Adicionar(Logradouro.Criar(0, GerarCep(), "Kaio", "Fernandes Branco", "SQLite", "SC", "Brasil").Value!);
        var a = Aluno.Criar(0, "Kaio", GerarCpf(), new DateOnly(2000, 1, 1), "48999999999", $"kaio{Guid.NewGuid():N}@teste.com", l, "1", "Fernandes Branco", "SenhaSQLite123", Arquivo.Criar([1]).Value!).Value!;
        return await _aluno.Adicionar(a);
    }
    [Fact] public async Task Aluno_crud_e_consultas_funcionam()
    {
        var a = await CriarAsync(); Assert.True(a.Id > 0); Assert.Equal("Kaio", (await _aluno.ObterPorId(a.Id))!.Nome); Assert.NotEmpty(await _aluno.ObterTodos());
        Assert.Equal(a.Id, (await _aluno.ObterPorCpf(a.Cpf))!.Id); Assert.Equal(a.Id, (await _aluno.ObterPorEmail(a.Email))!.Id); Assert.True(await _aluno.CpfJaExiste(a.Cpf)); Assert.False(await _aluno.CpfJaExiste(a.Cpf, a.Id)); Assert.True(await _aluno.EmailJaExiste(a.Email));
        Assert.True(await _aluno.TrocarSenha(a.Id, Senha.Criar("NovaSenhaSQLite123").Value!));
        var atualizado = Aluno.Criar(a.Id, "Kaio", a.Cpf.Valor, a.DataNascimento, a.Telefone.Valor, a.Email.Valor, a.Endereco.Logradouro, "2", "Fernandes Branco", "NovaSenhaSQLite123", a.Foto).Value!;
        await _aluno.Atualizar(atualizado); Assert.Equal("2", (await _aluno.ObterPorId(a.Id))!.Endereco.Numero); Assert.True(await _aluno.Remover(a.Id)); Assert.False(await _aluno.Remover(999999));
    }
}
