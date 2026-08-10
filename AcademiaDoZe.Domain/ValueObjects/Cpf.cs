// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Result<Cpf> Criar(string valor)
    {
        if (NormalizadoService.TextoVazioOuNulo(valor))
            return Result<Cpf>.Failure("Cpf", "CPF_OBRIGATORIO");

        var textoLimpo = NormalizadoService.LimparEDigitos(valor);

        if (textoLimpo.Length != 11)
            return Result<Cpf>.Failure("Cpf", "CPF_DIGITOS");

        if (!ValidarDigitosVerificadores(textoLimpo))
            return Result<Cpf>.Failure("Cpf", "CPF_INVALIDO");

        return Result<Cpf>.Success(new Cpf(textoLimpo));
    }

    private static bool ValidarDigitosVerificadores(string cpf)
    {
        // rejeita sequências repetidas, ex: 11111111111
        if (cpf.Distinct().Count() == 1)
            return false;

        var numeros = cpf.Select(c => c - '0').ToArray();

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += numeros[i] * (10 - i);
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;
        if (numeros[9] != digito1) return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += numeros[i] * (11 - i);
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;
        if (numeros[10] != digito2) return false;

        return true;
    }

    public override string ToString() => Valor;
}