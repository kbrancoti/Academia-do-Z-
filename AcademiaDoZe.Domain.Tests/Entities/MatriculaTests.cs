// Kaio Fernandes Branco
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class MatriculaTests
{
    private Aluno CriarAlunoValido()
    {
        var logradouro = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil").Value!;
        return Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-15)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null).Value!;
    }

    [Fact(DisplayName = "Matrícula: criação válida para aluno maior de idade")]
    public void Deve_Criar_Matricula_Para_Aluno_Maior_Idade()
    {
        var aluno = CriarAlunoValido();
        var result = Matricula.Criar(1, aluno, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Now), null, MatriculaRestricoes.None);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Matrícula: aluno menor de 16 requer laudo médico")]
    public void Deve_Falhar_Criacao_Quando_AlunoMenor16SemLaudo()
    {
        var logradouro = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil").Value!;
        var alunoMenor = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-14)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null).Value!;
        var result = Matricula.Criar(1, alunoMenor, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Now), null, MatriculaRestricoes.None);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Matrícula: com restrições médicas requer laudo")]
    public void Deve_Falhar_Criacao_Quando_RestricoesSemLaudo()
    {
        var aluno = CriarAlunoValido();
        var result = Matricula.Criar(1, aluno, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Now), null, MatriculaRestricoes.Diabetes);
        Assert.True(result.IsFailure);
    }

    [Theory(DisplayName = "Matrícula: diferentes planos")]
    [InlineData(MatriculaPlano.Mensal)]
    [InlineData(MatriculaPlano.Trimestral)]
    [InlineData(MatriculaPlano.Semestral)]
    [InlineData(MatriculaPlano.Anual)]
    public void Deve_Criar_Matricula_Com_Diferentes_Planos(MatriculaPlano plano)
    {
        var aluno = CriarAlunoValido();
        var result = Matricula.Criar(1, aluno, plano, DateOnly.FromDateTime(DateTime.Now), null, MatriculaRestricoes.None);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Matrícula: data fim calculada corretamente para plano mensal")]
    public void Deve_Calcular_DataFim_Corretamente_Plano_Mensal()
    {
        var aluno = CriarAlunoValido();
        var inicio = DateOnly.FromDateTime(DateTime.Now);
        var result = Matricula.Criar(1, aluno, MatriculaPlano.Mensal, inicio, null, MatriculaRestricoes.None);
        Assert.True(result.IsSuccess);
        var fim = result.Value!.DataFim;
        var esperado = inicio.AddMonths(1);
        Assert.Equal(esperado, fim);
    }

    [Fact(DisplayName = "Matrícula: com laudo médico e restrições")]
    public void Deve_Criar_Matricula_Com_Laudo_E_Restricoes()
    {
        var logradouro = Logradouro.Criar(1, "12345-678", "Rua Teste", "Bairro", "Cidade", "SP", "Brasil").Value!;
        var alunoMenor = Aluno.Criar(1, "João Silva", "529.982.247-25", DateOnly.FromDateTime(DateTime.Now.AddYears(-14)), 
            "(11) 91234-5678", "joao@example.com", logradouro, "100", "Apto 101", "Senha123", null).Value!;
        var laudoBytes = new byte[] { 1, 2, 3, 4, 5 };
        var laudo = Arquivo.Criar(laudoBytes).Value!;
        var result = Matricula.Criar(1, alunoMenor, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Now), laudo, 
            MatriculaRestricoes.Diabetes | MatriculaRestricoes.PressaoAlta);
        Assert.True(result.IsSuccess);
    }

    [Fact(DisplayName = "Matrícula: aluno nulo falha")]
    public void Deve_Falhar_Criacao_Quando_AlunoNulo()
    {
        var result = Matricula.Criar(1, null!, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Now), null, MatriculaRestricoes.None);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Matrícula: plano trimestral")]
    public void Deve_Calcular_DataFim_Corretamente_Plano_Trimestral()
    {
        var aluno = CriarAlunoValido();
        var inicio = DateOnly.FromDateTime(DateTime.Now);
        var result = Matricula.Criar(1, aluno, MatriculaPlano.Trimestral, inicio, null, MatriculaRestricoes.None);
        Assert.True(result.IsSuccess);
        var fim = result.Value!.DataFim;
        var esperado = inicio.AddMonths(3);
        Assert.Equal(esperado, fim);
    }
}
