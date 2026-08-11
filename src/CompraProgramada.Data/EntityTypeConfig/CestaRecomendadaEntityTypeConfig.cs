using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CestaRecomendadaEntityTypeConfig : EntityBaseConfiguration<CestaRecomendada>
{
    public override void Configure(EntityTypeBuilder<CestaRecomendada> builder)
    {
        base.Configure(builder);

        builder.ToTable("cesta");
        builder.Property(x => x.Nome).HasColumnName("nome").IsRequired().HasComment("nome cesta");
        builder.Property(x => x.Ativa).HasColumnName("ativa").IsRequired().HasDefaultValue(true).HasComment("cesta ativa");
        builder.HasMany(x => x.ComposicaoCesta).WithOne(x => x.Cesta).HasForeignKey(x => x.CestaId);
    }
}