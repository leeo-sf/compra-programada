using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CestaRecomendadaEntityTypeConfig : IEntityTypeConfiguration<CestaRecomendada>
{
    public void Configure(EntityTypeBuilder<CestaRecomendada> builder)
    {
        builder.ToTable("cesta");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.Nome).HasColumnName("nome").IsRequired().HasComment("nome cesta");
        builder.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired();
        builder.Property(x => x.DataDesativacao).HasColumnName("data_desativacao").IsRequired();
        builder.Property(x => x.Ativa).HasColumnName("ativa").IsRequired().HasDefaultValue(true).HasComment("cesta ativa");
        builder.HasMany(x => x.ComposicaoCesta).WithOne(x => x.Cesta).HasForeignKey(x => x.CestaId);
    }
}