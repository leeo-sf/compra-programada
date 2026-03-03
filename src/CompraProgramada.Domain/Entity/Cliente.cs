namespace CompraProgramada.Domain.Entity;

public record Cliente(int Id, string Nome, string Cpf, string Email, decimal ValorAnterior, decimal ValorMensal, DateTime DataAdesao, bool Ativo = true)
{
    public int Id { get; init; } = Id;
    public string Nome { get; init; } = Nome;
    public string Cpf { get; init; } = Cpf;
    public string Email { get; init; } = Email;
    public decimal ValorAnterior { get; init; } = ValorAnterior;
    public decimal ValorMensal { get; init; } = ValorMensal;
    public bool Ativo { get; init; } = Ativo;
    public DateTime DataAdesao{ get; init; } = DataAdesao;
    public ContaGrafica? ContaGrafica { get; init; } = default!;
}