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
