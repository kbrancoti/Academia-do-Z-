// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoColaboradorTests
{
    [Fact(DisplayName = "AcessoColaborador: criação válida dentro do horário")]
    public void Deve_Criar_AcessoColaborador_Dentro_Do_Horario()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 10, 0, 0);
        var result = AcessoColaborador.Criar(1, null!, hora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoColaborador: rejeita acesso antes das 6h")]
    public void Deve_Falhar_Criacao_Quando_AntesDas6h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 5, 59, 0);
        var result = AcessoColaborador.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "AcessoColaborador: rejeita acesso depois das 22h")]
    public void Deve_Falhar_Criacao_Quando_DepoisDas22h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 22, 1, 0);
        var result = AcessoColaborador.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }

    [Theory(DisplayName = "AcessoColaborador: horários válidos")]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(18)]
    [InlineData(21)]
    public void Deve_Criar_AcessoColaborador_Em_Horarios_Validos(int hora)
    {
        var agora = DateTime.Now;
        var dataHora = new DateTime(agora.Year, agora.Month, agora.Day, hora, 0, 0);
        var result = AcessoColaborador.Criar(1, null!, dataHora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoColaborador: exatamente às 6h é válido")]
    public void Deve_Criar_AcessoColaborador_Exatamente_6h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 6, 0, 0);
        var result = AcessoColaborador.Criar(1, null!, hora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoColaborador: exatamente às 22h é válido")]
    public void Deve_Criar_AcessoColaborador_Exatamente_22h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 22, 0, 0);
        var result = AcessoColaborador.Criar(1, null!, hora);
        Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "AcessoColaborador: múltiplos horários")]
    [InlineData(7)]
    [InlineData(9)]
    [InlineData(14)]
    [InlineData(19)]
    public void Deve_Criar_AcessoColaborador_Em_Varios_Horarios(int hora)
    {
        var agora = DateTime.Now;
        var dataHora = new DateTime(agora.Year, agora.Month, agora.Day, hora, 30, 0);
        var result = AcessoColaborador.Criar(1, null!, dataHora);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "AcessoColaborador: 0h é inválido")]
    public void Deve_Falhar_Criacao_Quando_0h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 0, 0, 0);
        var result = AcessoColaborador.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "AcessoColaborador: 23h é inválido")]
    public void Deve_Falhar_Criacao_Quando_23h()
    {
        var agora = DateTime.Now;
        var hora = new DateTime(agora.Year, agora.Month, agora.Day, 23, 0, 0);
        var result = AcessoColaborador.Criar(1, null!, hora);
        Assert.True(result.IsFailure);
    }
}
