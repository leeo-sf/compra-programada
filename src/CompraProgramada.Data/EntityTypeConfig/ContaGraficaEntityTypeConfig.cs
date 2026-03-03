using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class ContaGraficaEntityTypeConfig : IEntityTypeConfiguration<ContaGrafica>
{
    public void Configure(EntityTypeBuilder<ContaGrafica> builder)
    {
        builder.ToTable("conta_grafica");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.NumeroConta).HasColumnName("numero_conta").HasComment("número conta");
        builder.Property(x => x.DataCriacao).HasColumnName("data_criacao");
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasComment("tipo conta");
    }
}