// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public sealed class LogradouroRepository : BaseRepository, ILogradouroRepository
{
    private const string BaseSelect = "SELECT id_logradouro, cep, nome, bairro, cidade, estado, pais FROM tb_logradouro ";

    public LogradouroRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

    public async Task<Logradouro?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        await using var command = await CreateCommandAsync(BaseSelect + "WHERE id_logradouro = @Id", cancellationToken);
        command.AddParameter("@Id", id, DbType.Int32);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<IEnumerable<Logradouro>> ObterTodos(CancellationToken cancellationToken = default) =>
        await QueryManyAsync(BaseSelect + "ORDER BY nome", null, cancellationToken);

    public async Task<Logradouro> Adicionar(Logradouro entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        const string insert = "INSERT INTO tb_logradouro (cep, nome, bairro, cidade, estado, pais) VALUES (@Cep, @Nome, @Bairro, @Cidade, @Estado, @Pais)";
        try
        {
            await using var command = await CreateCommandAsync(FormatInsertQuery(insert), cancellationToken);
            AddEntityParameters(command, entity);
            var id = await command.ExecuteScalarIdAsync("ERRO_ADICIONAR_LOGRADOURO", "Falha ao obter ID do logradouro inserido.", cancellationToken);
            return CreateWithId(entity, id);
        }
        catch (DbException ex) { throw new InfrastructureException("ERRO_ADICIONAR_LOGRADOURO", "Erro ao adicionar logradouro.", ex); }
    }

    public async Task<Logradouro> Atualizar(Logradouro entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        const string sql = "UPDATE tb_logradouro SET cep=@Cep, nome=@Nome, bairro=@Bairro, cidade=@Cidade, estado=@Estado, pais=@Pais WHERE id_logradouro=@Id";
        try
        {
            await using var command = await CreateCommandAsync(sql, cancellationToken);
            AddEntityParameters(command, entity);
            command.AddParameter("@Id", entity.Id, DbType.Int32);
            if (await command.ExecuteNonQueryAsync(cancellationToken) == 0)
                throw new InfrastructureException("REGISTRO_NAO_ENCONTRADO", $"Nenhum logradouro encontrado com ID {entity.Id}.");
            return entity;
        }
        catch (DbException ex) { throw new InfrastructureException("ERRO_ATUALIZAR_LOGRADOURO", "Erro ao atualizar logradouro.", ex); }
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        await using var command = await CreateCommandAsync("DELETE FROM tb_logradouro WHERE id_logradouro=@Id", cancellationToken);
        command.AddParameter("@Id", id, DbType.Int32);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<Logradouro?> ObterPorCep(Cep cep, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cep);
        await using var command = await CreateCommandAsync(BaseSelect + "WHERE cep=@Cep", cancellationToken);
        command.AddParameter("@Cep", cep.Valor, DbType.String);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<bool> CepJaExiste(Cep cep, int? id = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cep);
        await using var command = await CreateCommandAsync("SELECT COUNT(1) FROM tb_logradouro WHERE cep=@Cep AND (@Id IS NULL OR id_logradouro<>@Id)", cancellationToken);
        command.AddParameter("@Cep", cep.Valor, DbType.String);
        command.AddParameter("@Id", id, DbType.Int32);
        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken)) > 0;
    }

    public Task<IEnumerable<Logradouro>> ObterPorCidade(string cidade, CancellationToken cancellationToken = default) =>
        QueryManyAsync(BaseSelect + "WHERE cidade=@Cidade ORDER BY bairro, nome", c => c.AddParameter("@Cidade", cidade, DbType.String), cancellationToken);

    public Task<IEnumerable<Logradouro>> ObterPorBairro(string cidade, string bairro, CancellationToken cancellationToken = default) =>
        QueryManyAsync(BaseSelect + "WHERE cidade=@Cidade AND bairro=@Bairro ORDER BY nome", c => { c.AddParameter("@Cidade", cidade, DbType.String); c.AddParameter("@Bairro", bairro, DbType.String); }, cancellationToken);

    private async Task<IEnumerable<Logradouro>> QueryManyAsync(string sql, Action<DbCommand>? configure, CancellationToken cancellationToken)
    {
        await using var command = await CreateCommandAsync(sql, cancellationToken);
        configure?.Invoke(command);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<Logradouro>();
        while (await reader.ReadAsync(cancellationToken)) result.Add(Map(reader));
        return result;
    }

    private static void AddEntityParameters(DbCommand command, Logradouro entity)
    {
        command.AddParameter("@Cep", entity.Cep.Valor, DbType.String); command.AddParameter("@Nome", entity.Nome, DbType.String);
        command.AddParameter("@Bairro", entity.Bairro, DbType.String); command.AddParameter("@Cidade", entity.Cidade, DbType.String);
        command.AddParameter("@Estado", entity.Estado, DbType.String); command.AddParameter("@Pais", entity.Pais, DbType.String);
    }

    private static Logradouro CreateWithId(Logradouro source, int id) => Logradouro.Criar(id, source.Cep.Valor, source.Nome, source.Bairro, source.Cidade, source.Estado, source.Pais).Value!;

    private static Logradouro Map(DbDataReader reader)
    {
        var result = Logradouro.Criar(reader.GetInt32Value("id_logradouro"), reader.GetStringValue("cep"), reader.GetStringValue("nome"), reader.GetStringValue("bairro"), reader.GetStringValue("cidade"), reader.GetStringValue("estado"), reader.GetStringValue("pais"));
        if (result.IsFailure) throw new InfrastructureException("ERRO_DOMINIO_MAPEAMENTO", string.Join(", ", result.Notifications.Select(n => n.Mensagem)));
        return result.Value!;
    }
}
