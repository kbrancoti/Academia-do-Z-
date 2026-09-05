// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public sealed class AlunoRepository : BaseRepository
{
    public AlunoRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

    public async Task<Aluno> Adicionar(Aluno entity, CancellationToken cancellationToken = default)
    {
        const string insert = "INSERT INTO tb_aluno (cpf, nome, nascimento, telefone, email, logradouro_id, numero, complemento, senha, foto) VALUES (@Cpf, @Nome, @Nascimento, @Telefone, @Email, @LogradouroId, @Numero, @Complemento, @Senha, @Foto)";
        try
        {
            await using var command = await CreateCommandAsync(FormatInsertQuery(insert), cancellationToken);
            AddParameters(command, entity);
            var id = await command.ExecuteScalarIdAsync("ERRO_ADICIONAR_ALUNO", "Falha ao obter ID do aluno inserido.", cancellationToken);
            return CreateWithId(entity, id);
        }
        catch (DbException ex) { throw new InfrastructureException("ERRO_ADICIONAR_ALUNO", "Erro ao adicionar aluno.", ex); }
    }

    public static Aluno Map(DbDataReader reader, string nameColumn = "nome")
    {
        var logradouro = Logradouro.Criar(reader.GetInt32Value("id_logradouro"), reader.GetStringValue("cep"), reader.GetStringValue("logradouro_nome"), reader.GetStringValue("bairro"), reader.GetStringValue("cidade"), reader.GetStringValue("estado"), reader.GetStringValue("pais")).Value!;
        var foto = Arquivo.Criar(reader.GetNullableBytes("foto") ?? []).Value!;
        var result = Aluno.Criar(reader.GetInt32Value("id_aluno"), reader.GetStringValue(nameColumn), reader.GetStringValue("cpf"), reader.GetDateOnlyValue("nascimento"), reader.GetStringValue("telefone"), reader.GetStringValue("email"), logradouro, reader.GetStringValue("numero"), reader.GetNullableString("complemento"), reader.GetStringValue("senha"), foto);
        if (result.IsFailure) throw new InfrastructureException("ERRO_DOMINIO_MAPEAMENTO", string.Join(", ", result.Notifications.Select(n => n.Mensagem)));
        return result.Value!;
    }

    private static void AddParameters(DbCommand command, Aluno entity)
    {
        command.AddParameter("@Cpf", entity.Cpf.Valor, DbType.String); command.AddParameter("@Nome", entity.Nome, DbType.String);
        command.AddParameter("@Nascimento", entity.DataNascimento, DbType.Date); command.AddParameter("@Telefone", entity.Telefone.Valor, DbType.String);
        command.AddParameter("@Email", entity.Email.Valor, DbType.String); command.AddParameter("@LogradouroId", entity.Endereco.Logradouro.Id, DbType.Int32);
        command.AddParameter("@Numero", entity.Endereco.Numero, DbType.String); command.AddParameter("@Complemento", entity.Endereco.Complemento, DbType.String);
        command.AddParameter("@Senha", entity.Senha.Valor, DbType.String); command.AddParameter("@Foto", entity.Foto.Conteudo, DbType.Binary);
    }

    private static Aluno CreateWithId(Aluno source, int id) => Aluno.Criar(id, source.Nome, source.Cpf.Valor, source.DataNascimento, source.Telefone.Valor, source.Email.Valor, source.Endereco.Logradouro, source.Endereco.Numero, source.Endereco.Complemento, source.Senha.Valor, source.Foto).Value!;
}
