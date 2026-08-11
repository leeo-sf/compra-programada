using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CustodiaFilhoteEntityTypeConfig : EntityBaseConfiguration<CustodiaFilhote>
{
    public override void Configure(EntityTypeBuilder<CustodiaFilhote> builder)
    {
        base.Configure(builder);

        builder.ToTable("custodia_filhote");
        builder.Property(x => x.ContaGraficaId).IsRequired().HasColumnName("conta_grafica_id").HasComment("identificador conta");
        builder.Property(x => x.Ticker).HasColumnName("ticker").HasComment("ativo");
        builder.Property(x => x.PrecoMedio).HasColumnName("preco_medio").HasDefaultValue(0).HasComment("custo médio ponderado de compra");
        builder.Property(x => x.Quantidade).HasColumnName("quantidade").HasDefaultValue(0).HasComment("quantidade comprado");
    }
}