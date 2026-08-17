// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;

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

    // método de fábrica, ponto de entrada para criar um objeto válido
    public static Result<AcessoAluno> Criar(int id, Aluno aluno, DateTime dataHora)
    {
        var notifications = new List<Notification>();

        if (aluno == null)
            notifications.Add(new Notification("Aluno", "ALUNO_OBRIGATORIO"));

        if (dataHora == default)
            notifications.Add(new Notification("DataHora", "DATA_HORA_OBRIGATORIO"));
        else
        {
            var hora = dataHora.Hour;
            if (hora < 6 || hora > 22)
                notifications.Add(new Notification("DataHora", "HORARIO_FORA_DO_PERMITIDO"));
        }

        if (notifications.Count != 0)
            return Result<AcessoAluno>.Failure(notifications);

        var acesso = new AcessoAluno(id, aluno!, dataHora);
        return Result<AcessoAluno>.Success(acesso);
    }
}
