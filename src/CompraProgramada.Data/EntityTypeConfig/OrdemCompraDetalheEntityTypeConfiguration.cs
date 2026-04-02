using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class OrdemCompraDetalheEntityTypeConfiguration : IEntityTypeConfiguration<OrdemCompraDetalhe>
{
    public void Configure(EntityTypeBuilder<OrdemCompraDetalhe> builder)
    {
        builder.ToTable("ordem_compra_detalhe");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.Tipo).IsRequired().HasColumnName("tipo").HasDefaultValue(OrdemCompraTipo.Fracionario).HasComment("tipo do lote");
        builder.Property(x => x.Ticker).IsRequired().HasColumnName("ticker").HasComment("ativo comprado");
        builder.Property(x => x.Quantidade).IsRequired().HasDefaultValue(0).HasColumnName("quantidade").HasComment("quantidade do lote");
        builder.Property(x => x.OrdemCompraId).IsRequired().HasColumnName("ordem_compra_id").HasComment("identificador ordem compra");
    }
}