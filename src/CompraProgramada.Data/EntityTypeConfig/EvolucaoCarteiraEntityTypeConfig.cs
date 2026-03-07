using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class EvolucaoCarteiraEntityTypeConfig : IEntityTypeConfiguration<EvolucaoCarteira>
{
    public void Configure(EntityTypeBuilder<EvolucaoCarteira> builder)
    {
        builder.ToTable("evolucao_carteira");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.Data).IsRequired().HasColumnName("data").HasComment("data da compra");
        builder.Property(x => x.ValorCarteira).IsRequired().HasColumnName("valor_carteira");
        builder.Property(x => x.ValorInvestido).IsRequired().HasColumnName("valor_investido");
        builder.Property(x => x.Rentabilidade).IsRequired().HasColumnName("rentabilidade");
        builder.Property(x => x.ContaGraficaId).IsRequired().HasColumnName("conta_grafica_id").HasComment("identificador conta");
    }
}