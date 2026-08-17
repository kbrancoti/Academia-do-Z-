// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class LogradouroTests
{
    [Fact(DisplayName = "Logradouro: criação válida")]
    public void Deve_Criar_Logradouro_Quando_Valido()
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil");
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Theory(DisplayName = "Logradouro: validação de CEP")]
    [InlineData("123")]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_CepInvalido(string cep)
    {
        var result = Logradouro.Criar(1, cep, "Rua Teste", "Bairro", "Cidade", "SP", "Brasil");
        Assert.True(result.IsFailure);
    }

    [Theory(DisplayName = "Logradouro: validação de nome")]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_NomeInvalido(string? nome)
    {
        var result = Logradouro.Criar(1, "12345-678", nome!, "Bairro", "Cidade", "SP", "Brasil");
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Logradouro: normaliza estado para maiúsculo")]
    public void Deve_Normalizar_Estado_Para_Maiusculo()
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "sp", "Brasil");
        Assert.True(result.IsSuccess);
        Assert.Equal("SP", result.Value!.Estado);
    }

    [Fact(DisplayName = "Logradouro: normaliza estado com espaços")]
    public void Deve_Normalizar_Estado_Remove_Espacos()
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "s p", "Brasil");
        Assert.True(result.IsSuccess);
        Assert.Equal("SP", result.Value!.Estado);
    }

    [Fact(DisplayName = "Logradouro: validação de bairro")]
    public void Deve_Falhar_Criacao_Quando_BairroVazio()
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "", "Cidade", "SP", "Brasil");
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Logradouro: validação de cidade")]
    public void Deve_Falhar_Criacao_Quando_CidadeVazia()
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "", "SP", "Brasil");
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Logradouro: validação de país")]
    public void Deve_Falhar_Criacao_Quando_PaisVazio()
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "");
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Logradouro: criação com CEP formatado")]
    public void Deve_Criar_Logradouro_Com_Cep_Formatado()
    {
        var result = Logradouro.Criar(1, "12345-678", "Avenida Paulista", "Bela Vista", "São Paulo", "SP", "Brasil");
        Assert.True(result.IsSuccess);
    }
}
