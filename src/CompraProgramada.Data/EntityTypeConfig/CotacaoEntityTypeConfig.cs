using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CotacaoEntityTypeConfig : EntityBaseConfiguration<Cotacao>
{
    public override void Configure(EntityTypeBuilder<Cotacao> builder)
    {
        base.Configure(builder);

        builder.ToTable("cotacao");
        builder.Property(x => x.DataPregao).IsRequired().HasColumnName("data_pregao").HasComment("data do pregão");
        builder.HasMany(x => x.ComposicaoCotacao).WithOne(x => x.Cotacao).HasForeignKey(x => x.CotacaoId);
    }
}