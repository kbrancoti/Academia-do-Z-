// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class MappingExtensions
{
    private static string Erro<T>(AcademiaDoZe.Domain.Common.Result<T> result) => string.Join(", ", result.Notifications.Select(n => n.Mensagem));

    public static LogradouroDto ToDto(this Logradouro value) => new() { Id = value.Id, Cep = value.Cep.Valor, Nome = value.Nome, Bairro = value.Bairro, Cidade = value.Cidade, Estado = value.Estado, Pais = value.Pais };
    public static Logradouro ToEntity(this LogradouroDto value)
    {
        var result = Logradouro.Criar(value.Id, value.Cep, value.Nome, value.Bairro, value.Cidade, value.Estado, value.Pais);
        return result.Value ?? throw new InvalidOperationException(Erro(result));
    }

    public static AlunoDto ToDto(this Aluno value) => new()
    {
        Id = value.Id, Nome = value.Nome, Cpf = value.Cpf.Valor, DataNascimento = value.DataNascimento,
        Telefone = value.Telefone.Valor, Email = value.Email.Valor, Endereco = value.Endereco.Logradouro.ToDto(),
        Numero = value.Endereco.Numero, Complemento = value.Endereco.Complemento, Foto = new ArquivoDto { Conteudo = value.Foto.Conteudo }, Senha = null
    };
    public static Aluno ToEntity(this AlunoDto value, Logradouro endereco, string senha)
    {
        var foto = Arquivo.Criar(value.Foto?.Conteudo ?? []).Value!;
        var result = Aluno.Criar(value.Id, value.Nome, value.Cpf, value.DataNascimento, value.Telefone, value.Email ?? string.Empty, endereco, value.Numero, value.Complemento ?? string.Empty, senha, foto);
        return result.Value ?? throw new InvalidOperationException(Erro(result));
    }

    public static ColaboradorDto ToDto(this Colaborador value) => new()
    {
        Id = value.Id, Nome = value.Nome, Cpf = value.Cpf.Valor, DataNascimento = value.DataNascimento,
        Telefone = value.Telefone.Valor, Email = value.Email.Valor, Endereco = value.Endereco.Logradouro.ToDto(),
        Numero = value.Endereco.Numero, Complemento = value.Endereco.Complemento, Foto = new ArquivoDto { Conteudo = value.Foto.Conteudo }, Senha = null,
        DataAdmissao = value.DataAdmissao, Tipo = value.Tipo.ToApplication(), Vinculo = value.Vinculo.ToApplication()
    };
    public static Colaborador ToEntity(this ColaboradorDto value, Logradouro endereco, string senha)
    {
        var foto = Arquivo.Criar(value.Foto?.Conteudo ?? []).Value!;
        var result = Colaborador.Criar(value.Id, value.Nome, value.Cpf, value.DataNascimento, value.Telefone, value.Email ?? string.Empty, endereco, value.Numero, value.Complemento ?? string.Empty, senha, foto, value.DataAdmissao, value.Tipo.ToDomain(), value.Vinculo.ToDomain());
        return result.Value ?? throw new InvalidOperationException(Erro(result));
    }

    public static MatriculaDto ToDto(this Matricula value) => new()
    {
        Id = value.Id, AlunoMatricula = value.AlunoMatricula.ToDto(), Plano = value.Plano.ToApplication(), DataInicio = value.DataInicio,
        DataFim = value.DataFim, Objetivo = value.Objetivo, RestricoesMedicas = value.RestricoesMedicas.ToApplication(),
        ObservacoesRestricoes = value.ObservacoesRestricoes, LaudoMedico = value.LaudoMedico is null ? null : new ArquivoDto { Conteudo = value.LaudoMedico.Conteudo }
    };
    public static Matricula ToEntity(this MatriculaDto value, Aluno aluno)
    {
        Arquivo? laudo = value.LaudoMedico is null ? null : Arquivo.Criar(value.LaudoMedico.Conteudo).Value;
        var result = Matricula.Criar(value.Id, aluno, value.Plano.ToDomain(), value.DataInicio, laudo, value.RestricoesMedicas.ToDomain(), value.Objetivo, value.ObservacoesRestricoes ?? string.Empty);
        return result.Value ?? throw new InvalidOperationException(Erro(result));
    }

    public static ColaboradorTipo ToDomain(this AppColaboradorTipo value) => (ColaboradorTipo)value;
    public static AppColaboradorTipo ToApplication(this ColaboradorTipo value) => (AppColaboradorTipo)value;
    public static ColaboradorVinculo ToDomain(this AppColaboradorVinculo value) => (ColaboradorVinculo)value;
    public static AppColaboradorVinculo ToApplication(this ColaboradorVinculo value) => (AppColaboradorVinculo)value;
    public static MatriculaPlano ToDomain(this AppMatriculaPlano value) => (MatriculaPlano)value;
    public static AppMatriculaPlano ToApplication(this MatriculaPlano value) => (AppMatriculaPlano)value;
    public static MatriculaRestricoes ToDomain(this AppMatriculaRestricoes value) => (MatriculaRestricoes)value;
    public static AppMatriculaRestricoes ToApplication(this MatriculaRestricoes value) => (AppMatriculaRestricoes)value;
}
