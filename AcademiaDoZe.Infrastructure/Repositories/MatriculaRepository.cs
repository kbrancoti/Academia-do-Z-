// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public sealed class MatriculaRepository : BaseRepository, IMatriculaRepository
{
    private const string BaseSelect = """
        SELECT m.id_matricula, m.aluno_id, m.plano, m.data_inicio, m.data_fim, m.objetivo, m.restricao_medica, m.obs_restricao, m.laudo_medico,
               a.id_aluno, a.cpf, a.nome AS aluno_nome, a.nascimento, a.telefone, a.email, a.logradouro_id, a.numero, a.complemento, a.senha, a.foto,
               l.id_logradouro, l.cep, l.nome AS logradouro_nome, l.bairro, l.cidade, l.estado, l.pais
        FROM tb_matricula m
        INNER JOIN tb_aluno a ON m.aluno_id = a.id_aluno
        INNER JOIN tb_logradouro l ON a.logradouro_id = l.id_logradouro
        """;

    public MatriculaRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

    public async Task<Matricula?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        await using var command = await CreateCommandAsync(BaseSelect + " WHERE m.id_matricula=@Id", cancellationToken);
        command.AddParameter("@Id", id, DbType.Int32);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public Task<IEnumerable<Matricula>> ObterTodos(CancellationToken cancellationToken = default) => QueryManyAsync(BaseSelect + " ORDER BY m.data_inicio DESC", null, cancellationToken);

    public async Task<Matricula> Adicionar(Matricula entity, CancellationToken cancellationToken = default)
    {
        const string insert = "INSERT INTO tb_matricula (aluno_id, plano, data_inicio, data_fim, objetivo, restricao_medica, obs_restricao, laudo_medico) VALUES (@AlunoId, @Plano, @DataInicio, @DataFim, @Objetivo, @RestricaoMedica, @Observacoes, @Laudo)";
        try
        {
            await using var command = await CreateCommandAsync(FormatInsertQuery(insert), cancellationToken);
            AddParameters(command, entity);
            var id = await command.ExecuteScalarIdAsync("ERRO_ADICIONAR_MATRICULA", "Falha ao obter ID da matrícula inserida.", cancellationToken);
            return CreateWithId(entity, id);
        }
        catch (DbException ex) { throw new InfrastructureException("ERRO_ADICIONAR_MATRICULA", "Erro ao adicionar matrícula.", ex); }
    }

    public async Task<Matricula> Atualizar(Matricula entity, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE tb_matricula SET aluno_id=@AlunoId, plano=@Plano, data_inicio=@DataInicio, data_fim=@DataFim, objetivo=@Objetivo, restricao_medica=@RestricaoMedica, obs_restricao=@Observacoes, laudo_medico=@Laudo WHERE id_matricula=@Id";
        try
        {
            await using var command = await CreateCommandAsync(sql, cancellationToken);
            AddParameters(command, entity); command.AddParameter("@Id", entity.Id, DbType.Int32);
            if (await command.ExecuteNonQueryAsync(cancellationToken) == 0) throw new InfrastructureException("REGISTRO_NAO_ENCONTRADO", $"Nenhuma matrícula encontrada com ID {entity.Id}.");
            return entity;
        }
        catch (DbException ex) { throw new InfrastructureException("ERRO_ATUALIZAR_MATRICULA", "Erro ao atualizar matrícula.", ex); }
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        await using var command = await CreateCommandAsync("DELETE FROM tb_matricula WHERE id_matricula=@Id", cancellationToken);
        command.AddParameter("@Id", id, DbType.Int32);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public Task<IEnumerable<Matricula>> ObterPorAluno(int alunoId, CancellationToken cancellationToken = default) => QueryManyAsync(BaseSelect + " WHERE m.aluno_id=@AlunoId ORDER BY m.data_inicio DESC", c => c.AddParameter("@AlunoId", alunoId, DbType.Int32), cancellationToken);

    public async Task<Matricula?> ObterMatriculaAtivaPorAluno(int alunoId, CancellationToken cancellationToken = default)
    {
        await using var command = await CreateCommandAsync(BaseSelect + $" WHERE m.aluno_id=@AlunoId AND m.data_fim >= {DbProvider.GetCurrentDateFunction(DatabaseType)} ORDER BY m.data_fim DESC", cancellationToken);
        command.AddParameter("@AlunoId", alunoId, DbType.Int32);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<bool> PossuiMatriculaAtiva(int alunoId, CancellationToken cancellationToken = default) => await ObterMatriculaAtivaPorAluno(alunoId, cancellationToken) is not null;
    public Task<IEnumerable<Matricula>> ObterAtivas(int alunoId = 0, CancellationToken cancellationToken = default) => QueryManyAsync(BaseSelect + $" WHERE m.data_fim >= {DbProvider.GetCurrentDateFunction(DatabaseType)}" + (alunoId > 0 ? " AND m.aluno_id=@AlunoId" : string.Empty) + " ORDER BY m.data_fim", alunoId > 0 ? c => c.AddParameter("@AlunoId", alunoId, DbType.Int32) : null, cancellationToken);
    public Task<IEnumerable<Matricula>> ObterVencendoEmDias(int dias, CancellationToken cancellationToken = default) => QueryManyAsync(BaseSelect + $" WHERE m.data_fim >= {DbProvider.GetCurrentDateFunction(DatabaseType)} AND m.data_fim <= {DbProvider.GetDateAddDaysExpression(DbProvider.GetCurrentDateFunction(DatabaseType), "@Dias", DatabaseType)} ORDER BY m.data_fim", c => c.AddParameter("@Dias", dias, DbType.Int32), cancellationToken);
    public Task<IEnumerable<Matricula>> ObterPorPlano(MatriculaPlano plano, CancellationToken cancellationToken = default) => QueryManyAsync(BaseSelect + " WHERE m.plano=@Plano ORDER BY a.nome", c => c.AddParameter("@Plano", (int)plano, DbType.Int32), cancellationToken);

    private async Task<IEnumerable<Matricula>> QueryManyAsync(string sql, Action<DbCommand>? configure, CancellationToken cancellationToken)
    {
        await using var command = await CreateCommandAsync(sql, cancellationToken); configure?.Invoke(command);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var items = new List<Matricula>(); while (await reader.ReadAsync(cancellationToken)) items.Add(Map(reader));
        return items;
    }

    private static void AddParameters(DbCommand command, Matricula entity)
    {
        command.AddParameter("@AlunoId", entity.AlunoMatricula.Id, DbType.Int32); command.AddParameter("@Plano", (int)entity.Plano, DbType.Int32);
        command.AddParameter("@DataInicio", entity.DataInicio, DbType.Date); command.AddParameter("@DataFim", entity.DataFim, DbType.Date);
        command.AddParameter("@Objetivo", entity.Objetivo, DbType.String); command.AddParameter("@RestricaoMedica", (int)entity.RestricoesMedicas, DbType.Int32);
        command.AddParameter("@Observacoes", entity.ObservacoesRestricoes, DbType.String); command.AddParameter("@Laudo", entity.LaudoMedico?.Conteudo, DbType.Binary);
    }

    private static Matricula CreateWithId(Matricula source, int id) => Matricula.Criar(id, source.AlunoMatricula, source.Plano, source.DataInicio, source.LaudoMedico, source.RestricoesMedicas, source.Objetivo, source.ObservacoesRestricoes).Value!;
    private static Matricula Map(DbDataReader reader)
    {
        var laudoBytes = reader.GetNullableBytes("laudo_medico"); var laudo = laudoBytes is null ? null : Arquivo.Criar(laudoBytes).Value;
        var result = Matricula.Criar(reader.GetInt32Value("id_matricula"), AlunoRepository.Map(reader, "aluno_nome"), (MatriculaPlano)reader.GetInt32Value("plano"), reader.GetDateOnlyValue("data_inicio"), laudo, (MatriculaRestricoes)reader.GetInt32Value("restricao_medica"), reader.GetStringValue("objetivo"), reader.GetNullableString("obs_restricao"));
        if (result.IsFailure) throw new InfrastructureException("ERRO_DOMINIO_MAPEAMENTO", string.Join(", ", result.Notifications.Select(n => n.Mensagem)));
        return result.Value!;
    }
}
