// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Tests.Services;

public class NormalizacaoServiceAdvancedTests
{
    [Theory(DisplayName = "LimparEspacos: textos com múltiplos espaços consecutivos")]
    [InlineData("a     b", "a b")]
    [InlineData("     a", "a")]
    [InlineData("a     ", "a")]
    public void Deve_Limpar_Espacos_Multiplos_Consecutivos(string input, string esperado)
    {
        var result = NormalizadoService.LimparEspacos(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "TextoVazioOuNulo: apenas espaços")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Deve_Considerar_Apenas_Espacos_Como_Vazio(string input)
    {
        var result = NormalizadoService.TextoVazioOuNulo(input);
        Assert.True(result);
    }

    [Theory(DisplayName = "ParaMaiusculo: números permanecem iguais")]
    [InlineData("123abc", "123ABC")]
    [InlineData("456DEF", "456DEF")]
    [InlineData("789ghi", "789GHI")]
    public void Deve_Manter_Numeros_Em_Maiusculo(string input, string esperado)
    {
        var result = NormalizadoService.ParaMaiusculo(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "LimparEDigitos: remove todos os números")]
    [InlineData("123abc456", "abc")]
    [InlineData("a1b2c3", "abc")]
    [InlineData("987654321", "")]
    public void Deve_Remover_Todos_Digitos(string input, string esperado)
    {
        var result = NormalizadoService.LimparEDigitos(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "LimparTodosEspacos: remove espaços do meio")]
    [InlineData("a b c d e", "abcde")]
    [InlineData("hello world", "helloworld")]
    public void Deve_Remover_Espacos_Do_Meio(string input, string esperado)
    {
        var result = NormalizadoService.LimparTodosEspacos(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "TextoVazioOuNulo: textos com conteúdo")]
    [InlineData("abc", false)]
    [InlineData("0", false)]
    [InlineData(" a ", false)]
    public void Deve_Considerar_Com_Conteudo_Como_Nao_Vazio(string input, bool esperado)
    {
        var result = NormalizadoService.TextoVazioOuNulo(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "ParaMaiusculo: caracteres especiais")]
    [InlineData("café", "CAFÉ")]
    [InlineData("ção", "ÇÃO")]
    public void Deve_Processar_Caracteres_Especiais(string input, string esperado)
    {
        var result = NormalizadoService.ParaMaiusculo(input);
        Assert.Equal(esperado, result);
    }

    [Theory(DisplayName = "LimparEspacos: preserva conteúdo")]
    [InlineData("a  b  c", "a b c")]
    [InlineData("x     y", "x y")]
    public void Deve_Preservar_Conteudo_Ao_Limpar_Espacos(string input, string esperado)
    {
        var result = NormalizadoService.LimparEspacos(input);
        Assert.Equal(esperado, result);
    }
}
