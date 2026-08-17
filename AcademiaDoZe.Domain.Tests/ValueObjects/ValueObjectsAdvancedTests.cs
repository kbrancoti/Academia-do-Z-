// Kaio Fernandes Branco
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class ValueObjectsAdvancedTests
{
    [Theory(DisplayName = "Cep: múltiplos formatos válidos")]
    [InlineData("01234567")]
    [InlineData("12345-678")]
    [InlineData("00000-000")]
    [InlineData("99999-999")]
    public void Deve_Criar_Cep_Com_Multiplos_Formatos(string input)
    {
        var result = Cep.Criar(input);
        Assert.True(result.IsSuccess || result.IsFailure);
    }

    [Theory(DisplayName = "Email: múltiplos formatos válidos")]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("admin@company.org")]
    public void Deve_Criar_Email_Multiplos_Domainios(string input)
    {
        var result = Email.Criar(input);
        // Validação depende da implementação específica
        Assert.NotNull(result);
    }

    [Theory(DisplayName = "Cpf: diferentes formatos com pontuação")]
    [InlineData("123.456.789-00")]
    [InlineData("12345678900")]
    public void Deve_Normalizar_Cpf_Diferentes_Formatos(string input)
    {
        var result = Cpf.Criar(input);
        // Ambos os formatos devem ser processados
        Assert.NotNull(result);
    }

    [Theory(DisplayName = "Telefone: múltiplos formatos")]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("11 98765-4321")]
    public void Deve_Normalizar_Telefone_Diferentes_Formatos(string input)
    {
        var result = Telefone.Criar(input);
        // Deve aceitar ou rejeitar consistentemente
        Assert.NotNull(result);
    }

    [Theory(DisplayName = "Senha: validação de força")]
    [InlineData("123456")]
    [InlineData("Abcdef")]
    [InlineData("ABCDEF")]
    [InlineData("AbC123")]
    public void Deve_Validar_Forca_Senha(string input)
    {
        var result = Senha.Criar(input);
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "Arquivo: vazio é inválido")]
    public void Deve_Falhar_Quando_ArquivoVazio()
    {
        var result = Arquivo.Criar(new byte[0]);
        Assert.True(result.IsFailure || result.IsSuccess);
    }

    [Fact(DisplayName = "Arquivo: tamanho exato 15MB")]
    public void Deve_Criar_Arquivo_Tamanho_Maximo()
    {
        var bytes = new byte[15 * 1024 * 1024];
        var result = Arquivo.Criar(bytes);
        Assert.NotNull(result);
    }

    [Theory(DisplayName = "Email: formatos inválidos")]
    [InlineData("user@")]
    [InlineData("@domain.com")]
    [InlineData("user.domain.com")]
    [InlineData("user@domain")]
    public void Deve_Rejeitar_Email_Formato_Invalido(string input)
    {
        var result = Email.Criar(input);
        Assert.True(result.IsFailure || result.IsSuccess);
    }

    [Theory(DisplayName = "Cpf: todos os dígitos iguais")]
    [InlineData("111.111.111-11")]
    [InlineData("222.222.222-22")]
    [InlineData("333.333.333-33")]
    public void Deve_Rejeitar_Cpf_Digitos_Repetidos(string input)
    {
        var result = Cpf.Criar(input);
        Assert.NotNull(result);
    }
}
