using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class DistribuicaoEntityTypeConfiguration : IEntityTypeConfiguration<Distribuicao>
{
    public void Configure(EntityTypeBuilder<Distribuicao> builder)
    {
        builder.ToTable("distribuicao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.OrdemCompraId).IsRequired().HasColumnName("ordem_compra_id").HasComment("identificador ordem de compra");
        builder.Property(x => x.ContaGraficaId).IsRequired().HasColumnName("conta_grafica_id").HasComment("identificador conta cliente");
        builder.Property(x => x.Ticker).IsRequired().HasColumnName("ticker").HasComment("ativo comprado");
        builder.Property(x => x.QuantidadeAlocada).IsRequired().HasColumnName("quantidade_alocada").HasComment("quantidade alocada na custodia");
        builder.Property(x => x.ValorOperacao).IsRequired().HasColumnName("valor_operacao").HasComment("resultado financeiro da fatia");
        builder.HasOne(x => x.OrdemCompra).WithMany(x => x.Distribuicoes).HasForeignKey(x => x.OrdemCompraId);
        builder.HasOne(x => x.ContaGrafica).WithMany(x => x.Distribuicoes).HasForeignKey(x => x.ContaGraficaId);
    }
}