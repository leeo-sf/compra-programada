using CompraProgramada.Domain.Entity.BaseEntity;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;

namespace CompraProgramada.Domain.Entity;

public class Cliente : EntidadeBase
{
    public string Nome { get; private set; } = default!;
    public string Cpf { get; } = default!;
    public string Email { get; private set; } = default!;
    public decimal ValorAnterior { get; private set; }
    public decimal ValorMensal { get; private set; }
    public bool Ativo { get; private set; }
    public ContaGrafica ContaGrafica { get; private set; } = default!;
    public decimal ValorAporte => ValorMensal / 3;
    private const decimal VALOR_MINIMO_ADESAO = 100;

    private Cliente() { }

    internal Cliente(int id, string nome, string cpf, string email, decimal valorAnterior, decimal valorMensal, bool ativo, DateTime dataAdesao, DateTime dataAtualizacao, ContaGrafica contaGrafica)
        : base(id, dataAdesao, dataAtualizacao)
    {
        Nome = nome;
        Cpf = cpf;
        Email = email;
        ValorAnterior = valorAnterior;
        ValorMensal = valorMensal;
        Ativo = ativo;
        ContaGrafica = contaGrafica;
    }

    internal Cliente(string nome, string cpf, string email, decimal valorMensal) : base(0)
    {
        if (valorMensal < VALOR_MINIMO_ADESAO)
            throw new ValorMensalException(VALOR_MINIMO_ADESAO);

        Nome = nome;
        Cpf = cpf;
        Email = email;
        ValorAnterior = valorMensal;
        ValorMensal = valorMensal;
        Ativo = true;
    }

    public static Cliente Criar(AdesaoRequest request)
        => new(request.Nome, request.Cpf, request.Email, request.ValorMensal);

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