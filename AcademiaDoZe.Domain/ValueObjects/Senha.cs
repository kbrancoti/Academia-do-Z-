// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Result<Senha> Criar(string valor)
    {
        if (NormalizadoService.TextoVazioOuNulo(valor))
            return Result<Senha>.Failure("Senha", "SENHA_OBRIGATORIA");

        if (valor.Length < 6)
            return Result<Senha>.Failure("Senha", "SENHA_TAMANHO_MINIMO");

        if (!valor.Any(char.IsDigit))
            return Result<Senha>.Failure("Senha", "SENHA_DEVE_CONTER_NUMERO");

        if (!valor.Any(char.IsLetter))
            return Result<Senha>.Failure("Senha", "SENHA_DEVE_CONTER_LETRA");

        // Nota: aqui normalmente armazenaríamos um hash (ex: BCrypt), não a senha em texto puro.
        // Deixei em texto puro para manter a camada Domain sem dependências externas,
        // mas vale considerar mover o hashing para a camada de Application/Infra.
        return Result<Senha>.Success(new Senha(valor));
    }

    public override string ToString() => "****";
}