// Kaio Fernandes Branco
// Testes do Serviço de Normalização - Academia do Zé
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Tests.Services;

public class NormalizacaoServiceTests
{
    [Theory(DisplayName = "TextoVazioOuNulo: valida textos vazios e nulos")]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData("texto", false)]
    public void Deve_Validar_TextoVazioOuNulo(string? input, bool esperado)
    {
        var result = NormalizadoService.TextoVazioOuNulo(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "LimparEspacos: remove espaços múltiplos")]
    [InlineData("texto   com    espacos", "texto com espacos")]
    [InlineData("  inicio", "inicio")]
    [InlineData("fim  ", "fim")]
    [InlineData("  ambos  ", "ambos")]
    public void Deve_Limpar_Espacos_Multiplos(string input, string esperado)
    {
        var result = NormalizadoService.LimparEspacos(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "LimparTodosEspacos: remove todos os espaços")]
    [InlineData("texto com espacos", "textocomEspacos")]
    [InlineData("123 456", "123456")]
    [InlineData("a b c", "abc")]
    public void Deve_Limpar_Todos_Espacos(string input, string esperado)
    {
        var result = NormalizadoService.LimparTodosEspacos(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "ParaMaiusculo: normaliza para maiúsculas")]
    [InlineData("texto", "TEXTO")]
    [InlineData("MiXtO", "MIXTO")]
    [InlineData("123", "123")]
    public void Deve_Converter_Para_Maiusculo(string input, string esperado)
    {
        var result = NormalizadoService.ParaMaiusculo(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "LimparEDigitos: remove dígitos de uma string")]
    [InlineData("abc123def", "abcdef")]
    [InlineData("123", "")]
    [InlineData("nodigitos", "nodigitos")]
    public void Deve_Limpar_Digitos(string input, string esperado)
    {
        var result = NormalizadoService.LimparEDigitos(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "Combinação de operações")]
    [InlineData("   Texto  MIXTO   123   ", "TEXTOXXMIXTO123")]
    public void Deve_Combinar_Operacoes(string input, string esperado)
    {
        var intermediate = NormalizadoService.LimparEspacos(input);
        intermediate = NormalizadoService.ParaMaiusculo(intermediate);
        intermediate = NormalizadoService.LimparTodosEspacos(intermediate);
        Assert.Equal(esperado, intermediate);
    }
}
