// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class ColaboradorTests
{
    private Logradouro CriarLogradouroValido()
    {
        return Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil").Value!;
    }

    [Fact(DisplayName = "Colaborador: criação válida como CLT")]
    public void Deve_Criar_Colaborador_Quando_Valido()
    {
        var logradouro = CriarLogradouroValido();
        var result = Colaborador.Criar(1, "Maria Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "maria@example.com", logradouro, "100", "Apto 101", "Senha123", null, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), ColaboradorTipo.Administrador, ColaboradorVinculo.CLT);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Colaborador: idade mínima 12 anos")]
    public void Deve_Falhar_Criacao_Quando_IdadeMenor12Anos()
    {
        var logradouro = CriarLogradouroValido();
        var result = Colaborador.Criar(1, "Maria Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-10)), 
            "(11) 91234-5678", "maria@example.com", logradouro, "100", "Apto 101", "Senha123", null, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), ColaboradorTipo.Administrador, ColaboradorVinculo.CLT);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Colaborador: data de admissão não pode ser no futuro")]
    public void Deve_Falhar_Criacao_Quando_DataAdmissaoNoFuturo()
    {
        var logradouro = CriarLogradouroValido();
        var result = Colaborador.Criar(1, "Maria Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "maria@example.com", logradouro, "100", "Apto 101", "Senha123", null, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(30)), ColaboradorTipo.Administrador, ColaboradorVinculo.CLT);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Colaborador: Administrador deve ser CLT")]
    public void Deve_Falhar_Criacao_Quando_AdministradorComVinculoEstagio()
    {
        var logradouro = CriarLogradouroValido();
        var result = Colaborador.Criar(1, "Maria Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "maria@example.com", logradouro, "100", "Apto 101", "Senha123", null, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), ColaboradorTipo.Administrador, ColaboradorVinculo.Estagio);
        Assert.True(result.IsFailure);
    }

    [Theory(DisplayName = "Colaborador: tipos diferentes (Atendente, Instrutor)")]
    [InlineData(ColaboradorTipo.Atendente)]
    [InlineData(ColaboradorTipo.Instrutor)]
    public void Deve_Criar_Colaborador_Com_Tipos_Diferentes(ColaboradorTipo tipo)
    {
        var logradouro = CriarLogradouroValido();
        var result = Colaborador.Criar(1, "Maria Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "maria@example.com", logradouro, "100", "Apto 101", "Senha123", null, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), tipo, ColaboradorVinculo.CLT);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Colaborador: com foto")]
    public void Deve_Criar_Colaborador_Com_Foto()
    {
        var logradouro = CriarLogradouroValido();
        var fotoBytes = new byte[] { 1, 2, 3, 4, 5 };
        var foto = Arquivo.Criar(fotoBytes).Value!;
        var result = Colaborador.Criar(1, "Maria Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "maria@example.com", logradouro, "100", "Apto 101", "Senha123", foto, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), ColaboradorTipo.Atendente, ColaboradorVinculo.CLT);
        Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "Colaborador: vínculo diferente (Estágio)")]
    [InlineData(ColaboradorVinculo.Estagio)]
    public void Deve_Criar_Colaborador_Com_Vinculo_Estagio(ColaboradorVinculo vinculo)
    {
        var logradouro = CriarLogradouroValido();
        var result = Colaborador.Criar(1, "João Estagiário", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(-10)), ColaboradorTipo.Instrutor, vinculo);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Colaborador: email inválido")]
    public void Deve_Falhar_Criacao_Quando_EmailInvalido()
    {
        var logradouro = CriarLogradouroValido();
        var result = Colaborador.Criar(1, "Maria Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), 
            "(11) 91234-5678", "emailinvalido", logradouro, "100", "Apto 101", "Senha123", null, 
            DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), ColaboradorTipo.Atendente, ColaboradorVinculo.CLT);
        Assert.True(result.IsFailure);
    }
}
