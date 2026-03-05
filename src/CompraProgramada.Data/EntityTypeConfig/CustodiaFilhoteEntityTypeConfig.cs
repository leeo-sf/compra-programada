using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CustodiaFilhoteEntityTypeConfig : IEntityTypeConfiguration<CustodiaFilhote>
{
    public void Configure(EntityTypeBuilder<CustodiaFilhote> builder)
    {
        builder.ToTable("custodia_filhote");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.ContaGraficaId).IsRequired().HasColumnName("conta_grafica_id").HasComment("identificador conta");
        builder.Property(x => x.Ticker).HasColumnName("ticker").HasComment("ativo");
        builder.Property(x => x.Quantidade).HasColumnName("quantidade").HasDefaultValue(0).HasComment("quantidade comprado");
    }
}