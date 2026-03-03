using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CotacaoEntityTypeConfig : IEntityTypeConfiguration<Cotacao>
{
    public void Configure(EntityTypeBuilder<Cotacao> builder)
    {
        builder.ToTable("cotacao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.DataPregao).IsRequired().HasColumnName("data_pregao").HasComment("data do pregão");
        builder.HasMany(x => x.ComposicaoCotacao).WithOne(x => x.Cotacao).HasForeignKey(x => x.CotacaoId);
    }
}