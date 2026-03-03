using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class ClienteEntityTypeConfig : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("cliente");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.Nome).HasColumnName("nome").IsRequired().HasComment("nome usuário");
        builder.HasIndex(x => x.Cpf).IsUnique();
        builder.Property(x => x.Cpf).HasColumnName("cpf").IsRequired().HasMaxLength(11).HasComment("cpf do usuário");
        builder.Property(x => x.Email).HasColumnName("email").IsRequired().HasComment("email usuário");
        builder.Property(x => x.ValorAnterior).HasColumnName("valor_anterior").IsRequired().HasComment("valor anterior de compra");
        builder.Property(x => x.ValorMensal).HasColumnName("valor_mensal").IsRequired().HasComment("valor mensal de compra");
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired().HasDefaultValue(true).HasComment("usuário ativo");
        builder.Property(x => x.DataAdesao).HasColumnName("data_adesao").IsRequired();
        builder.HasOne(x => x.ContaGrafica).WithOne(x => x.Cliente).HasForeignKey<ContaGrafica>(x => x.ClienteId);
    }
}