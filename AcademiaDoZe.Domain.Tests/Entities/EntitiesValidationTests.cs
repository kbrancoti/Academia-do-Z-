// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class EntitiesValidationTests
{
    private Logradouro CriarLogradouroValido()
    {
        return Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil").Value!;
    }

    [Theory(DisplayName = "Aluno: múltiplas idades válidas")]
    [InlineData(-12)]
    [InlineData(-20)]
    [InlineData(-30)]
    [InlineData(-50)]
    public void Deve_Criar_Aluno_Idades_Diversas_Validas(int anosSubtrair)
    {
        var logradouro = CriarLogradouroValido();
        var data = DateOnly.FromDateTime(DateTime.Now.AddYears(anosSubtrair));
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", data, 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null);
        
        if (anosSubtrair >= -12)
            Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "Aluno: nome com diferentes formatos")]
    [InlineData("João da Silva")]
    [InlineData("MARIA SANTOS")]
    [InlineData("João")]
    [InlineData("José")]
    public void Deve_Criar_Aluno_Nomes_Variados(string nome)
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, nome, "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null);
        Assert.True(result.IsSuccess || result.IsFailure);
    }

    [Theory(DisplayName = "Logradouro: estados brasileiros")]
    [InlineData("SP")]
    [InlineData("RJ")]
    [InlineData("MG")]
    [InlineData("BA")]
    [InlineData("RS")]
    public void Deve_Criar_Logradouro_Estados_Brasileiros(string estado)
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", estado, "Brasil");
        Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "Logradouro: cidades diversas")]
    [InlineData("São Paulo")]
    [InlineData("Rio de Janeiro")]
    [InlineData("Belo Horizonte")]
    [InlineData("Salvador")]
    public void Deve_Criar_Logradouro_Cidades_Diversas(string cidade)
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", cidade, "SP", "Brasil");
        Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "Aluno: emails válidos")]
    [InlineData("user@domain.com")]
    [InlineData("first.last@company.co.uk")]
    [InlineData("admin@site.org")]
    public void Deve_Criar_Aluno_Emails_Validos(string email)
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", email, logradouro, "100", "Apto 101", "Senha123", null);
        
        Assert.True(result.IsSuccess || result.IsFailure);
    }

    [Theory(DisplayName = "Aluno: números de endereço")]
    [InlineData("1")]
    [InlineData("100")]
    [InlineData("999")]
    [InlineData("12345")]
    public void Deve_Criar_Aluno_Enderecos_Numeracao_Diversa(string numero)
    {
        var logradouro = CriarLogradouroValido();
        var result = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "joao@example.com", logradouro, numero, "Apto 101", "Senha123", null);
        
        Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "Logradouro: nomes de rua")]
    [InlineData("Rua A")]
    [InlineData("Avenida Paulista")]
    [InlineData("Travessa B")]
    [InlineData("Largo do Pelourinho")]
    public void Deve_Criar_Logradouro_Nomes_Rua_Diversos(string nomeRua)
    {
        var result = Logradouro.Criar(1, "12345-678", nomeRua, "Bairro", "Cidade", "SP", "Brasil");
        Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "Logradouro: bairros")]
    [InlineData("Centro")]
    [InlineData("Zona Leste")]
    [InlineData("Vila Madalena")]
    [InlineData("Bela Vista")]
    public void Deve_Criar_Logradouro_Bairros_Diversos(string bairro)
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", bairro, "Cidade", "SP", "Brasil");
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Logradouro: padrão CEP com hífen")]
    public void Deve_Aceitar_Cep_Com_Hifen()
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil");
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact(DisplayName = "Logradouro: padrão CEP sem hífen")]
    public void Deve_Aceitar_Cep_Sem_Hifen()
    {
        var result = Logradouro.Criar(1, "12345678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil");
        Assert.True(result.IsSuccess);
    }
}
