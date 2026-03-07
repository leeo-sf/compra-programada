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
        builder.Property(x => x.ContaGraficaId).IsRequired().HasColumnName("conta_grafica_id").HasComment("identificador conta");
        builder.Property(x => x.Ticker).IsRequired().HasColumnName("ticker").HasComment("ativo comprado");
        builder.Property(x => x.Quantidade).IsRequired().HasColumnName("quantidade").HasComment("quantidade comprado");
        builder.Property(x => x.ValorAporte).IsRequired().HasColumnName("valor_aporte").HasComment("valor aporte");
        builder.Property(x => x.PrecoExecutado).IsRequired().HasColumnName("preco_executado").HasComment("preco de fechamento do ativo");
        builder.Property(x => x.PrecoMedio).IsRequired().HasColumnName("preco_medio").HasComment("preco medio");
    }
}