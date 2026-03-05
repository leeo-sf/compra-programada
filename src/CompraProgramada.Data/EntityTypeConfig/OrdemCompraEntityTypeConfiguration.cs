using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class OrdemCompraEntityTypeConfiguration : IEntityTypeConfiguration<OrdemCompra>
{
    public void Configure(EntityTypeBuilder<OrdemCompra> builder)
    {
        builder.ToTable("ordem_compra");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.Ticker).IsRequired().HasColumnName("ticker").HasComment("ativo comprado");
        builder.Property(x => x.QuantidadeLotePadrao).IsRequired().HasDefaultValue(0).HasColumnName("quantidade_lote_padrao").HasComment("quantos multiplos de 100 foram comprados");
        builder.Property(x => x.Quantidade).IsRequired().HasColumnName("quantidade").HasComment("quantidade total de ativos comprados");
        builder.Property(x => x.PrecoExecucao).IsRequired().HasColumnName("preco_execucao").HasComment("preço da compra executada");
        builder.Property(x => x.Data).HasColumnName("data").IsRequired().HasComment("data da compra");
    }
}