using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class ComposicaoCestaEntityTypeConfig : IEntityTypeConfiguration<ComposicaoCesta>
{
    public void Configure(EntityTypeBuilder<ComposicaoCesta> builder)
    {
        builder.ToTable("composicao_cesta");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.CestaId).HasColumnName("cesta_id").IsRequired().HasComment("identificador cesta");
        builder.Property(x => x.Ticker).HasColumnName("ticker").IsRequired().HasComment("ativo");
        builder.Property(x => x.Percentual).HasColumnName("percentual").IsRequired().HasComment("percentual ocupante na cesta");
    }
}