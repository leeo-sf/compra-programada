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
        builder.Property(x => x.QuantidadeTotal).IsRequired().HasDefaultValue(0).HasColumnName("quantidade_total").HasComment("quantos total de ativos");
        builder.Property(x => x.PrecoUnitario).IsRequired().HasColumnName("preco_unitatio").HasComment("preço de fechamento de cada ativo");
        builder.Property(x => x.ValorTotal).IsRequired().HasColumnName("valor_total").HasComment("valor total da ordem de compra");
        builder.Property(x => x.Data).HasColumnName("data").IsRequired().HasComment("data da compra");
        builder.HasMany(x => x.Detalhes).WithOne(x => x.OrdemCompra).HasForeignKey(x => x.OrdemCompraId);
    }
}