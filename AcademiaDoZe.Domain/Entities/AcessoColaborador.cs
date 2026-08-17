// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Common;

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

    // método de fábrica, ponto de entrada para criar um objeto válido
    public static Result<AcessoColaborador> Criar(int id, Colaborador colaborador, DateTime dataHora)
    {
        var notifications = new List<Notification>();

        if (colaborador == null)
            notifications.Add(new Notification("Colaborador", "COLABORADOR_OBRIGATORIO"));

        if (dataHora == default)
            notifications.Add(new Notification("DataHora", "DATA_HORA_OBRIGATORIO"));
        else
        {
            var hora = dataHora.Hour;
            if (hora < 6 || hora > 22)
                notifications.Add(new Notification("DataHora", "HORARIO_FORA_DO_PERMITIDO"));
        }

        if (notifications.Count != 0)
            return Result<AcessoColaborador>.Failure(notifications);

        var acesso = new AcessoColaborador(id, colaborador!, dataHora);
        return Result<AcessoColaborador>.Success(acesso);
    }
}
