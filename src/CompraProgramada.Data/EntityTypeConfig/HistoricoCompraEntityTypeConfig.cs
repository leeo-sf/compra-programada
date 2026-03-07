using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class HistoricoCompraEntityTypeConfig : IEntityTypeConfiguration<HistoricoCompra>
{
    public void Configure(EntityTypeBuilder<HistoricoCompra> builder)
    {
        builder.ToTable("historico_compra");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.Data).IsRequired().HasColumnName("data").HasComment("data da compra");
        builder.Property(x => x.Valor).IsRequired().HasColumnName("valor").HasComment("valor compra");
        builder.Property(x => x.ContaGraficaId).HasColumnName("conta_grafica_id").IsRequired().HasComment("identificador conta");
    }
}