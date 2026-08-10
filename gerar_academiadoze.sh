#!/bin/bash
# Gera a estrutura de pastas e arquivos da camada AcademiaDoZe.Domain
# baseado no material "03 de 20" (DDD / Clean Architecture)
# Uso: bash gerar_academiadoze.sh
set -e

ROOT="AcademiaDoZe.Domain"

mkdir -p "$ROOT/Common"
mkdir -p "$ROOT/Entities"
mkdir -p "$ROOT/Enums"
mkdir -p "$ROOT/Exceptions"
mkdir -p "$ROOT/Repositories"
mkdir -p "$ROOT/Services"
mkdir -p "$ROOT/ValueObjects"

# ---------------------------------------------------------------
# Common
# ---------------------------------------------------------------

cat > "$ROOT/Common/Notification.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Common;

public record Notification(string Propriedade, string Mensagem);
EOF

cat > "$ROOT/Common/Result.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Common;

public class Result<T>
{
    public T? Value { get; }
    public IReadOnlyCollection<Notification> Notifications { get; }

    public bool IsSuccess => Notifications.Count == 0;
    public bool IsFailure => Notifications.Count != 0;

    private Result(T? value, IEnumerable<Notification> notifications)
    {
        Value = value;
        Notifications = notifications.ToList().AsReadOnly();
    }

    public static Result<T> Success(T value) => new(value, []);
    public static Result<T> Failure(IEnumerable<Notification> notifications) => new(default, notifications);
    public static Result<T> Failure(string propriedade, string mensagem) => new(default, [new Notification(propriedade, mensagem)]);
    public static Result<T> Failure(Notification notification) => new(default, [notification]);
}
EOF

# ---------------------------------------------------------------
# Enums
# ---------------------------------------------------------------

cat > "$ROOT/Enums/ColaboradorTipo.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Enums;

public enum ColaboradorTipo
{
    Administrador = 0,
    Atendente = 1,
    Instrutor = 2
}
EOF

cat > "$ROOT/Enums/ColaboradorVinculo.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Enums;

public enum ColaboradorVinculo
{
    CLT = 0,
    Estagio = 1
}
EOF

cat > "$ROOT/Enums/MatriculaPlano.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Enums;

public enum MatriculaPlano
{
    Mensal = 0,
    Trimestral = 1,
    Semestral = 2,
    Anual = 3
}
EOF

cat > "$ROOT/Enums/MatriculaRestricoes.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Enums;

[Flags]
public enum MatriculaRestricoes
{
    None = 0,
    Diabetes = 1,
    PressaoAlta = 2,
    Labirintite = 4,
    Alergias = 8,
    ProblemasRespiratorios = 16,
    RemedioContinuo = 32
}
EOF

# ---------------------------------------------------------------
# Exceptions
# ---------------------------------------------------------------

cat > "$ROOT/Exceptions/DomainException.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Exceptions;

// classe base para exceções de domínio
// permitindo exceções específicas de regras de negócio
// uso de construtor primário para simplificar a criação de exceções com mensagem
// sealed para evitar herança adicional, mantendo a hierarquia de exceções clara
public sealed class DomainException(string message) : Exception(message)
{
}
EOF

# ---------------------------------------------------------------
# Services
# ---------------------------------------------------------------

cat > "$ROOT/Services/NormalizadoService.cs" << 'EOF'
// Kaio Fernandes Branco
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Services;

public static partial class NormalizadoService
{
    // verifica se o texto é nulo ou vazio
    public static bool TextoVazioOuNulo(string? texto) => string.IsNullOrWhiteSpace(texto);

    // remove espaços repetidos e espaços no início e no final do texto
    public static string LimparEspacos(string? texto) => string.IsNullOrWhiteSpace(texto) ? string.Empty : EspacosRegex().Replace(texto, " ").Trim();

    // limpa todos os espaços
    public static string LimparTodosEspacos(string? texto) => string.IsNullOrWhiteSpace(texto) ? string.Empty : texto.Replace(" ", string.Empty);

    // converte o texto para maiúsculo
    public static string ParaMaiusculo(string? texto) => string.IsNullOrEmpty(texto) ? string.Empty : texto.ToUpperInvariant();

    // manter somente digitos numericos
    public static string LimparEDigitos(string? texto) => string.IsNullOrEmpty(texto) ? string.Empty : new string([.. texto.Where(char.IsDigit)]);

    [GeneratedRegex(@"\s+")]
    private static partial Regex EspacosRegex();
}
EOF

# ---------------------------------------------------------------
# ValueObjects
# ---------------------------------------------------------------

cat > "$ROOT/ValueObjects/Cep.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cep
{
    public string Valor { get; }

    private Cep(string valor)
    {
        Valor = valor;
    }

    public static Result<Cep> Criar(string valor)
    {
        if (NormalizadoService.TextoVazioOuNulo(valor))
            return Result<Cep>.Failure("Cep", "CEP_OBRIGATORIO");

        var textoLimpo = NormalizadoService.LimparEDigitos(valor);
        if (textoLimpo.Length != 8)
            return Result<Cep>.Failure("Cep", "CEP_DIGITOS");

        return Result<Cep>.Success(new Cep(textoLimpo));
    }

    public override string ToString() => Valor;
}
EOF

cat > "$ROOT/ValueObjects/Email.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Valor { get; }

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Result<Email> Criar(string valor)
    {
        var textoLimpo = NormalizadoService.LimparEspacos(valor);
        if (string.IsNullOrWhiteSpace(textoLimpo) || !ValidarFormato(textoLimpo))
            return Result<Email>.Failure("Email", "EMAIL_FORMATO");

        return Result<Email>.Success(new Email(textoLimpo));
    }

    private static bool ValidarFormato(string email)
    {
        var partes = email.Split('@');
        if (partes.Length != 2) return false;
        if (string.IsNullOrWhiteSpace(partes[0])) return false;

        var dominio = partes[1];
        if (string.IsNullOrWhiteSpace(dominio)) return false;
        if (dominio.StartsWith('.') || dominio.EndsWith('.')) return false;

        var labels = dominio.Split('.');
        if (labels.Length < 2) return false;
        if (labels.Any(l => string.IsNullOrWhiteSpace(l))) return false;

        return true;
    }

    public override string ToString() => Valor;
}
EOF

cat > "$ROOT/ValueObjects/Endereco.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Endereco
{
    public Logradouro Logradouro { get; }
    public string Numero { get; }
    public string Complemento { get; }

    private Endereco(Logradouro logradouro, string numero, string complemento)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
    }

    public static Result<Endereco> Criar(Logradouro logradouro, string numero, string complemento)
    {
        var notifications = new List<Notification>();

        if (logradouro == null)
            notifications.Add(new Notification("Endereco", "LOGRADOURO_OBRIGATORIO"));

        if (NormalizadoService.TextoVazioOuNulo(numero))
            notifications.Add(new Notification("Numero", "NUMERO_OBRIGATORIO"));
        else
            numero = NormalizadoService.LimparEspacos(numero);

        complemento = NormalizadoService.LimparEspacos(complemento);

        if (notifications.Count != 0)
            return Result<Endereco>.Failure(notifications);

        return Result<Endereco>.Success(new Endereco(logradouro!, numero, complemento));
    }
}
EOF

cat > "$ROOT/ValueObjects/Arquivo.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public byte[] Conteudo { get; }

    private Arquivo(byte[] conteudo)
    {
        Conteudo = conteudo;
    }

    public static Result<Arquivo> Criar(byte[] conteudo)
    {
        if (conteudo == null)
            return Result<Arquivo>.Failure("Arquivo", "ARQUIVO_OBRIGATORIO");

        const int tamanhoMaximoBytes = 15 * 1024 * 1024; // 15MB
        if (conteudo.Length > tamanhoMaximoBytes)
            return Result<Arquivo>.Failure("Arquivo", "ARQUIVO_TIPO_TAMANHO");

        // cria e retorna o objeto
        return Result<Arquivo>.Success(new Arquivo(conteudo));
    }
}
EOF

cat > "$ROOT/ValueObjects/Telefone.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Telefone
{
    public string Valor { get; }

    private Telefone(string valor)
    {
        Valor = valor;
    }

    public static Result<Telefone> Criar(string valor)
    {
        if (NormalizadoService.TextoVazioOuNulo(valor))
            return Result<Telefone>.Failure("Telefone", "TELEFONE_OBRIGATORIO");

        var textoLimpo = NormalizadoService.LimparEDigitos(valor);
        if (textoLimpo.Length != 11)
            return Result<Telefone>.Failure("Telefone", "TELEFONE_DIGITOS");

        return Result<Telefone>.Success(new Telefone(textoLimpo));
    }

    public override string ToString() => Valor;
}
EOF

# Cpf.cs e Senha.cs: apenas o esqueleto inicial aparece no material.
# O Factory Method (Criar) segue o mesmo padrão de Cep/Telefone/Email
# e fica como parte do laboratório prático a ser implementado por você.

cat > "$ROOT/ValueObjects/Cpf.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    // TODO: implementar o método de fábrica Criar (normalização + validação de CPF),
    // seguindo o mesmo padrão de Cep/Telefone/Email (Result<Cpf>, Notification).
}
EOF

cat > "$ROOT/ValueObjects/Senha.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    // TODO: implementar o método de fábrica Criar (normalização + validação de senha),
    // seguindo o mesmo padrão de Cep/Telefone/Email (Result<Senha>, Notification).
}
EOF

# ---------------------------------------------------------------
# Entities
# ---------------------------------------------------------------

cat > "$ROOT/Entities/Entity.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Domain.Entities;

// Classe base para todas as entidades, garantindo identidade única e validação de Id
public abstract class Entity
{
    public int Id { get; protected set; }

    protected Entity(int id = 0)
    {
        if (id < 0) throw new DomainException("ID_NEGATIVO");
        Id = id;
    }
}
EOF

cat > "$ROOT/Entities/Logradouro.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public sealed class Logradouro : Entity
{
    // encapsulamento das propriedades, aplicando imutabilidade
    public Cep Cep { get; }
    public string Nome { get; }
    public string Bairro { get; }
    public string Cidade { get; }
    public string Estado { get; }
    public string Pais { get; }

    // construtor privado para evitar instância direta
    private Logradouro(int id, Cep cep, string nome, string bairro, string cidade, string estado, string pais) : base(id)
    {
        Cep = cep;
        Nome = nome;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        Pais = pais;
    }

    public static Result<Logradouro> Criar(int id, string cep, string nome, string bairro, string cidade, string estado, string pais)
    {
        var notifications = new List<Notification>();

        var cepResult = Cep.Criar(cep);
        if (cepResult.IsFailure)
            notifications.AddRange(cepResult.Notifications);

        if (NormalizadoService.TextoVazioOuNulo(nome))
            notifications.Add(new Notification("Nome", "NOME_OBRIGATORIO"));
        else
            nome = NormalizadoService.LimparEspacos(nome);

        if (NormalizadoService.TextoVazioOuNulo(bairro))
            notifications.Add(new Notification("Bairro", "BAIRRO_OBRIGATORIO"));
        else
            bairro = NormalizadoService.LimparEspacos(bairro);

        if (NormalizadoService.TextoVazioOuNulo(cidade))
            notifications.Add(new Notification("Cidade", "CIDADE_OBRIGATORIO"));
        else
            cidade = NormalizadoService.LimparEspacos(cidade);

        if (NormalizadoService.TextoVazioOuNulo(estado))
            notifications.Add(new Notification("Estado", "ESTADO_OBRIGATORIO"));
        else
            estado = NormalizadoService.ParaMaiusculo(NormalizadoService.LimparTodosEspacos(estado));

        if (NormalizadoService.TextoVazioOuNulo(pais))
            notifications.Add(new Notification("Pais", "PAIS_OBRIGATORIO"));
        else
            pais = NormalizadoService.LimparEspacos(pais);

        if (notifications.Count != 0)
            return Result<Logradouro>.Failure(notifications);

        return Result<Logradouro>.Success(new Logradouro(id, cepResult.Value!, nome, bairro, cidade, estado, pais));
    }
}
EOF

cat > "$ROOT/Entities/Pessoa.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public abstract class Pessoa : Entity
{
    public string Nome { get; protected set; }
    public Cpf Cpf { get; protected set; }
    public DateOnly DataNascimento { get; protected set; }
    public Telefone Telefone { get; protected set; }
    public Email Email { get; protected set; }
    public Endereco Endereco { get; protected set; }
    public Senha Senha { get; protected set; }
    public Arquivo Foto { get; protected set; }

    protected Pessoa(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email,
        Endereco endereco, Senha senha, Arquivo foto) : base(id)
    {
        Nome = nome;
        Cpf = cpf;
        DataNascimento = dataNascimento;
        Telefone = telefone;
        Email = email;
        Endereco = endereco;
        Senha = senha;
        Foto = foto;
    }
}
EOF

cat > "$ROOT/Entities/Colaborador.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa
{
    // encapsulamento das propriedades, aplicando imutabilidade
    public DateOnly DataAdmissao { get; private set; }
    public ColaboradorTipo Tipo { get; private set; }
    public ColaboradorVinculo Vinculo { get; private set; }

    // construtor privado para evitar instância direta
    private Colaborador(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Endereco endereco, Senha senha, Arquivo foto,
        DateOnly dataAdmissao, ColaboradorTipo tipo, ColaboradorVinculo vinculo) : base(id, nome, cpf, dataNascimento, telefone, email, endereco, senha, foto)
    {
        DataAdmissao = dataAdmissao;
        Tipo = tipo;
        Vinculo = vinculo;
    }

    // método de fábrica, ponto de entrada para criar um objeto válido
    public static Result<Colaborador> Criar(int id, string nome, string cpf, DateOnly dataNascimento, string telefone, string email, Logradouro endereco, string numero, string complemento,
        string senha, Arquivo foto, DateOnly dataAdmissao, ColaboradorTipo tipo, ColaboradorVinculo vinculo)
    {
        var notifications = new List<Notification>();

        // Validações e normalizações
        if (NormalizadoService.TextoVazioOuNulo(nome))
            notifications.Add(new Notification("Nome", "NOME_OBRIGATORIO"));
        else
            nome = NormalizadoService.LimparEspacos(nome);

        if (dataNascimento == default)
            notifications.Add(new Notification("DataNascimento", "DATA_NASCIMENTO_OBRIGATORIO"));
        else if (dataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-12)))
            notifications.Add(new Notification("DataNascimento", "DATA_NASCIMENTO_MINIMA_INVALIDA"));

        if (dataAdmissao == default)
            notifications.Add(new Notification("DataAdmissao", "DATA_ADMISSAO_OBRIGATORIO"));
        else if (dataAdmissao > DateOnly.FromDateTime(DateTime.Today))
            notifications.Add(new Notification("DataAdmissao", "DATA_ADMISSAO_MAIOR_ATUAL"));

        if (!Enum.IsDefined(tipo))
            notifications.Add(new Notification("Tipo", "TIPO_COLABORADOR_INVALIDO"));

        if (!Enum.IsDefined(vinculo))
            notifications.Add(new Notification("Vinculo", "VINCULO_COLABORADOR_INVALIDO"));

        if (Enum.IsDefined(tipo) && Enum.IsDefined(vinculo) && tipo == ColaboradorTipo.Administrador && vinculo != ColaboradorVinculo.CLT)
            notifications.Add(new Notification("Vinculo", "ADMINISTRADOR_CLT_INVALIDO"));

        // Instanciação e validação via Value Objects
        var cpfResult = Cpf.Criar(cpf);
        if (cpfResult.IsFailure) notifications.AddRange(cpfResult.Notifications);

        var telefoneResult = Telefone.Criar(telefone);
        if (telefoneResult.IsFailure) notifications.AddRange(telefoneResult.Notifications);

        var emailResult = Email.Criar(email);
        if (emailResult.IsFailure) notifications.AddRange(emailResult.Notifications);

        var senhaResult = Senha.Criar(senha);
        if (senhaResult.IsFailure) notifications.AddRange(senhaResult.Notifications);

        var enderecoResult = Endereco.Criar(endereco, numero, complemento);
        if (enderecoResult.IsFailure) notifications.AddRange(enderecoResult.Notifications);

        if (notifications.Count != 0)
            return Result<Colaborador>.Failure(notifications);

        // criação e retorno do objeto
        var colaborador = new Colaborador(id, nome, cpfResult.Value!, dataNascimento, telefoneResult.Value!, emailResult.Value!, enderecoResult.Value!, senhaResult.Value!, foto, dataAdmissao, tipo, vinculo);
        return Result<Colaborador>.Success(colaborador);
    }
}
EOF

# Aluno, Matricula, AcessoAluno, AcessoColaborador: no material só aparece
# o esqueleto inicial (construtor privado, sem Factory Method completo).
# Implementar o método estático Criar(...) faz parte do laboratório prático.

cat > "$ROOT/Entities/Aluno.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa
{
    // construtor privado para evitar instância direta
    private Aluno(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email,
        Endereco endereco, Senha senha, Arquivo foto)
        : base(id, nome, cpf, dataNascimento, telefone, email, endereco, senha, foto)
    {
    }

    // TODO: implementar o método de fábrica estático Criar(...),
    // seguindo o mesmo padrão usado em Colaborador.Criar(...).
}
EOF

cat > "$ROOT/Entities/Matricula.cs" << 'EOF'
// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity
{
    // encapsulamento das propriedades, aplicando imutabilidade
    public Aluno AlunoMatricula { get; private set; }
    public MatriculaPlano Plano { get; private set; }
    public DateOnly DataInicio { get; private set; }
    public DateOnly DataFim { get; private set; }
    public string Objetivo { get; private set; }
    public MatriculaRestricoes RestricoesMedicas { get; private set; }
    public string ObservacoesRestricoes { get; private set; }
    public Arquivo? LaudoMedico { get; private set; }

    // construtor privado para evitar instância direta
    private Matricula(int id, Aluno alunoMatricula, MatriculaPlano plano, DateOnly dataInicio, DateOnly dataFim,
        string objetivo, MatriculaRestricoes restricoesMedicas, Arquivo? laudoMedico, string observacoesRestricoes = "") : base(id)
    {
        AlunoMatricula = alunoMatricula;
        Plano = plano;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Objetivo = objetivo;
        RestricoesMedicas = restricoesMedicas;
        LaudoMedico = laudoMedico;
        ObservacoesRestricoes = observacoesRestricoes;
    }

    // TODO: implementar o método de fábrica estático Criar(...),
    // seguindo o mesmo padrão usado em Colaborador.Criar(...).
}
EOF

cat > "$ROOT/Entities/AcessoColaborador.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity
{
    public Colaborador Colaborador { get; private set; }
    public DateTime DataHora { get; private set; }

    private AcessoColaborador(int id, Colaborador colaborador, DateTime dataHora) : base(id)
    {
        Colaborador = colaborador;
        DataHora = dataHora;
    }

    // TODO: implementar o método de fábrica estático Criar(...).
}
EOF

cat > "$ROOT/Entities/AcessoAluno.cs" << 'EOF'
// Kaio Fernandes Branco
namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity
{
    public Aluno Aluno { get; private set; }
    public DateTime DataHora { get; private set; }

    private AcessoAluno(int id, Aluno aluno, DateTime dataHora) : base(id)
    {
        Aluno = aluno;
        DataHora = dataHora;
    }

    // TODO: implementar o método de fábrica estático Criar(...).
}
EOF

echo "Estrutura gerada com sucesso em: $(pwd)/$ROOT"
find "$ROOT" -type f | sort
