// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoAlunoTests
{
    [Fact(DisplayName = "AcessoAluno: criação válida dentro do horário")]
    public void Deve_Criar_AcessoAluno_Dentro_Do_Horario()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 10, 0, 0);
        var result = AcessoAluno.Criar(1, null!, hora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoAluno: rejeita acesso antes das 6h")]
    public void Deve_Falhar_Criacao_Quando_AntesDas6h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 5, 59, 0);
        var result = AcessoAluno.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "AcessoAluno: rejeita acesso depois das 22h")]
    public void Deve_Falhar_Criacao_Quando_DepoisDas22h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 22, 1, 0);
        var result = AcessoAluno.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }

    [Theory(DisplayName = "AcessoAluno: horários válidos")]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(18)]
    [InlineData(21)]
    public void Deve_Criar_AcessoAluno_Em_Horarios_Validos(int hora)
    {
        var agora = DateTime.Now;
        var dataHora = new DateTime(agora.Year, agora.Month, agora.Day, hora, 0, 0);
        var result = AcessoAluno.Criar(1, null!, dataHora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoAluno: exatamente às 6h é válido")]
    public void Deve_Criar_AcessoAluno_Exatamente_6h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 6, 0, 0);
        var result = AcessoAluno.Criar(1, null!, hora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoAluno: exatamente às 22h é válido")]
    public void Deve_Criar_AcessoAluno_Exatamente_22h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 22, 0, 0);
        var result = AcessoAluno.Criar(1, null!, hora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoAluno: 5h59min é inválido")]
    public void Deve_Falhar_Criacao_Quando_5h59min()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 5, 59, 59);
        var result = AcessoAluno.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "AcessoAluno: 22h1min é inválido")]
    public void Deve_Falhar_Criacao_Quando_22h1min()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 22, 1, 0);
        var result = AcessoAluno.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }
}
