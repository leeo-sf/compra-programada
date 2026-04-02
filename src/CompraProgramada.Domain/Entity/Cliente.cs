using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;

namespace CompraProgramada.Domain.Entity;

public class Cliente
{
    public int Id { get; init; }
    public string Nome { get; private set; } = default!;
    public string Cpf { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public decimal ValorAnterior { get; private set; }
    public decimal ValorMensal { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataAdesao{ get; private set; }
    public ContaGrafica ContaGrafica { get; private set; } = default!;
    public decimal ValorAporte => ValorMensal / 3;
    private const decimal VALOR_MINIMO_ADESAO = 100;

    private Cliente() { }

    internal Cliente(int id, string nome, string cpf, string email, decimal valorAnterior, decimal valorMensal, bool ativo, DateTime dataAdesao, ContaGrafica contaGrafica)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        Email = email;
        ValorAnterior = valorAnterior;
        ValorMensal = valorMensal;
        Ativo = ativo;
        DataAdesao = dataAdesao;
        ContaGrafica = contaGrafica;
    }

    internal Cliente(int id, string nome, string cpf, string email, decimal valorAnterior, decimal valorMensal, bool ativo, DateTime dataAdesao)
    {
        if (valorMensal < VALOR_MINIMO_ADESAO)
            throw new ValorMensalException(VALOR_MINIMO_ADESAO);

        Id = id;
        Nome = nome;
        Cpf = cpf;
        Email = email;
        ValorAnterior = valorAnterior;
        ValorMensal = valorMensal;
        Ativo = ativo;
        DataAdesao = dataAdesao;
    }

    public static Cliente Criar(AdesaoRequest request)
        => new Cliente(0, request.Nome, request.Cpf, request.Email, request.ValorMensal, request.ValorMensal, true, DateTime.Now);

    public void AtualizarValorMensal(AtualizarValorMensalRequest request)
    {
        if (request.NovoValorMensal < VALOR_MINIMO_ADESAO)
            throw new ValorMensalException(VALOR_MINIMO_ADESAO);

        ValorMensal = request.NovoValorMensal;
    }

    public void Desativar() => Ativo = false;

    public void AdicionarConta(ContaGrafica conta)
        => ContaGrafica = conta;
}