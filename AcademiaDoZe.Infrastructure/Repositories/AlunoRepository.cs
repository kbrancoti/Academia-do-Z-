// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public sealed class AlunoRepository : BaseRepository, IAlunoRepository
{
    private const string SelectSql = "SELECT a.id_aluno,a.cpf,a.nome,a.nascimento,a.telefone,a.email,a.logradouro_id,a.numero,a.complemento,a.senha,a.foto,l.id_logradouro,l.cep,l.nome AS logradouro_nome,l.bairro,l.cidade,l.estado,l.pais FROM tb_aluno a INNER JOIN tb_logradouro l ON a.logradouro_id=l.id_logradouro";
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

    public async Task<Aluno?> ObterPorId(int id, CancellationToken ct = default) => await ObterUm(SelectSql + " WHERE a.id_aluno=@Id", c => c.AddParameter("@Id", id, DbType.Int32), ct);
    public Task<IEnumerable<Aluno>> ObterTodos(CancellationToken ct = default) => ObterVarios(SelectSql + " ORDER BY a.nome", null, ct);
    public Task<Aluno?> ObterPorCpf(Cpf cpf, CancellationToken ct = default) => ObterUm(SelectSql + " WHERE a.cpf=@V", c => c.AddParameter("@V", cpf.Valor, DbType.String), ct);
    public Task<Aluno?> ObterPorEmail(Email email, CancellationToken ct = default) => ObterUm(SelectSql + " WHERE a.email=@V", c => c.AddParameter("@V", email.Valor, DbType.String), ct);
    public async Task<Aluno> Atualizar(Aluno e, CancellationToken ct = default) { const string q="UPDATE tb_aluno SET cpf=@Cpf,nome=@Nome,nascimento=@Nascimento,telefone=@Telefone,email=@Email,logradouro_id=@LogradouroId,numero=@Numero,complemento=@Complemento,senha=@Senha,foto=@Foto WHERE id_aluno=@Id"; await using var c=await CreateCommandAsync(q,ct); AddParameters(c,e); c.AddParameter("@Id",e.Id,DbType.Int32); if(await c.ExecuteNonQueryAsync(ct)==0) throw new InfrastructureException("REGISTRO_NAO_ENCONTRADO","Aluno não encontrado."); return e; }
    public async Task<bool> Remover(int id,CancellationToken ct=default){await using var c=await CreateCommandAsync("DELETE FROM tb_aluno WHERE id_aluno=@Id",ct);c.AddParameter("@Id",id,DbType.Int32);return await c.ExecuteNonQueryAsync(ct)>0;}
    public Task<bool> CpfJaExiste(Cpf v,int? id=null,CancellationToken ct=default)=>Existe("cpf",v.Valor,id,ct); public Task<bool> EmailJaExiste(Email v,int? id=null,CancellationToken ct=default)=>Existe("email",v.Valor,id,ct);
    public async Task<bool> TrocarSenha(int id,Senha s,CancellationToken ct=default){await using var c=await CreateCommandAsync("UPDATE tb_aluno SET senha=@S WHERE id_aluno=@Id",ct);c.AddParameter("@S",s.Valor,DbType.String);c.AddParameter("@Id",id,DbType.Int32);return await c.ExecuteNonQueryAsync(ct)>0;}
    private async Task<bool> Existe(string coluna,string valor,int? id,CancellationToken ct){await using var c=await CreateCommandAsync($"SELECT COUNT(1) FROM tb_aluno WHERE {coluna}=@V AND (@Id IS NULL OR id_aluno<>@Id)",ct);c.AddParameter("@V",valor,DbType.String);c.AddParameter("@Id",id,DbType.Int32);return Convert.ToInt32(await c.ExecuteScalarAsync(ct))>0;}
    private async Task<Aluno?> ObterUm(string q,Action<DbCommand> setup,CancellationToken ct){await using var c=await CreateCommandAsync(q,ct);setup(c);await using var r=await c.ExecuteReaderAsync(ct);return await r.ReadAsync(ct)?Map(r):null;}
    private async Task<IEnumerable<Aluno>> ObterVarios(string q,Action<DbCommand>? setup,CancellationToken ct){await using var c=await CreateCommandAsync(q,ct);setup?.Invoke(c);await using var r=await c.ExecuteReaderAsync(ct);var itens=new List<Aluno>();while(await r.ReadAsync(ct))itens.Add(Map(r));return itens;}
}
