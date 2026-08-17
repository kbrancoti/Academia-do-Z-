// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;
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

    // método de fábrica, ponto de entrada para criar um objeto válido
    public static Result<Matricula> Criar(int id, Aluno aluno, MatriculaPlano plano, DateOnly dataInicio, 
        Arquivo? laudo, MatriculaRestricoes restricoes, string objetivo = "", string observacoes = "")
    {
        var notifications = new List<Notification>();

        if (aluno == null)
            notifications.Add(new Notification("Aluno", "ALUNO_OBRIGATORIO"));

        if (!Enum.IsDefined(plano))
            notifications.Add(new Notification("Plano", "PLANO_MATRICULA_INVALIDO"));

        if (dataInicio == default)
            notifications.Add(new Notification("DataInicio", "DATA_INICIO_OBRIGATORIO"));

        // Validação: menor de 16 anos requer laudo médico
        if (aluno != null && aluno.DataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)))
        {
            if (laudo == null)
                notifications.Add(new Notification("LaudoMedico", "LAUDO_MEDICO_OBRIGATORIO_MENOR_16"));
        }

        // Validação: se possui restrições médicas, precisa de laudo
        if (restricoes != MatriculaRestricoes.None && laudo == null)
            notifications.Add(new Notification("LaudoMedico", "LAUDO_MEDICO_OBRIGATORIO_COM_RESTRICOES"));

        if (notifications.Count != 0)
            return Result<Matricula>.Failure(notifications);

        // Calcular data fim baseado no plano
        var dataFim = plano switch
        {
            MatriculaPlano.Mensal => dataInicio.AddMonths(1),
            MatriculaPlano.Trimestral => dataInicio.AddMonths(3),
            MatriculaPlano.Semestral => dataInicio.AddMonths(6),
            MatriculaPlano.Anual => dataInicio.AddYears(1),
            _ => dataInicio
        };

        var matricula = new Matricula(id, aluno, plano, dataInicio, dataFim, objetivo, restricoes, laudo, observacoes);
        return Result<Matricula>.Success(matricula);
    }
}
