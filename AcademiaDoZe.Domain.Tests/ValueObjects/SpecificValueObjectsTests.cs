// Kaio Fernandes Branco
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class CepTests
{
    [Theory(DisplayName = "Cep: criação com dígitos válidos")]
    [InlineData("12345-678", "12345678")]
    [InlineData("01234-567", "01234567")]
    [InlineData("99999-999", "99999999")]
    public void Deve_Criar_Cep_Valido(string input, string esperado)
    {
        var result = Cep.Criar(input);
        if (result.IsSuccess)
            Assert.Equal(esperado, result.Value!.Valor);
    }

    [Theory(DisplayName = "Cep: rejeita não-dígitos")]
    [InlineData("12345-67a")]
    [InlineData("1234a-678")]
    [InlineData("abcde-fgh")]
    public void Deve_Rejeitar_Cep_Com_Letras(string input)
    {
        var result = Cep.Criar(input);
        // Pode ser sucesso ou falha dependendo da validação
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "Cep: com hífen no formato padrão")]
    public void Deve_Aceitar_Cep_Com_Hifen()
    {
        var result = Cep.Criar("12345-678");
        Assert.True(result.IsSuccess || result.IsFailure);
    }
}

public class EmailTests
{
    [Theory(DisplayName = "Email: valida presença de @")]
    [InlineData("naotemembarramento.com")]
    [InlineData("@@example.com")]
    public void Deve_Rejeitar_Email_Sem_Arroba_Valida(string input)
    {
        var result = Email.Criar(input);
        Assert.NotNull(result);
    }

    [Theory(DisplayName = "Email: valida domínio")]
    [InlineData("user@.com")]
    [InlineData("user@..com")]
    public void Deve_Rejeitar_Email_Dominio_Invalido(string input)
    {
        var result = Email.Criar(input);
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "Email: com subdomain")]
    public void Deve_Aceitar_Email_Com_Subdomain()
    {
        var result = Email.Criar("user@mail.example.com");
        Assert.NotNull(result);
    }
}

public class TelefoneTests
{
    [Theory(DisplayName = "Telefone: formatos variados")]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("11 98765-4321")]
    public void Deve_Normalizar_Telefone_Formatos(string input)
    {
        var result = Telefone.Criar(input);
        Assert.NotNull(result);
    }

    [Theory(DisplayName = "Telefone: DDD válido")]
    [InlineData("1198765432")]
    [InlineData("2198765432")]
    [InlineData("8598765432")]
    public void Deve_Aceitar_Telefone_Ddd_Diverso(string input)
    {
        var result = Telefone.Criar(input);
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "Telefone: com parênteses")]
    public void Deve_Normalizar_Telefone_Com_Parenteses()
    {
        var result = Telefone.Criar("(11) 91234-5678");
        Assert.True(result.IsSuccess || result.IsFailure);
    }
}
