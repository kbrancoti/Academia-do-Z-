// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AlunoTests
{
    private Logradouro CriarLogradouroValido()
    {
        return Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil").Value!;
    }

    [Fact(DisplayName = "Aluno: criação válida")]
    public void Deve_Criar_Aluno_Quando_Valido()
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact(DisplayName = "Aluno: idade mínima 12 anos")]
    public void Deve_Falhar_Criacao_Quando_IdadeMenor12Anos()
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-10)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null);
        Assert.True(result.IsFailure);
    }

    [Theory(DisplayName = "Aluno: validação de nome")]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_NomeInvalido(string? nome)
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, nome!, "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Aluno: CPF inválido")]
    public void Deve_Falhar_Criacao_Quando_CpfInvalido()
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "000.000.000-00", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Aluno: email inválido")]
    public void Deve_Falhar_Criacao_Quando_EmailInvalido()
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "(11) 91234-5678", "emailinvalido", logradouro, "100", "Apto 101", "Senha123", null);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Aluno: telefone inválido")]
    public void Deve_Falhar_Criacao_Quando_TelefoneInvalido()
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "123", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Aluno: dados completos com foto")]
    public void Deve_Criar_Aluno_Com_Foto()
    {
        var logradouro = CriarLogradouroValido();
        var fotoBytes = new byte[] { 1, 2, 3, 4, 5 };
        var foto = Arquivo.Criar(fotoBytes).Value!;
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", foto);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Aluno: senha inválida")]
    public void Deve_Falhar_Criacao_Quando_SenhaInvalida()
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "senhafraca", null);
        Assert.True(result.IsFailure);
    }
}
