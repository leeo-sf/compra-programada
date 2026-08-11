using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class ContaGraficaEntityTypeConfig : EntityBaseContaConfiguration<ContaGrafica>
{
    public override void Configure(EntityTypeBuilder<ContaGrafica> builder)
    {
        base.Configure(builder);

        builder.ToTable("conta_grafica");
        builder.Property(x => x.ClienteId).HasColumnName("cliente_id").HasComment("identificador cliente");
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasComment("tipo conta");
        builder.HasMany(x => x.CustodiaFilhotes).WithOne(x => x.ContaGrafica).HasForeignKey(x => x.ContaGraficaId);
        builder.HasMany(x => x.HistoricoCompra).WithOne(x => x.ContaGrafica).HasForeignKey(x => x.ContaGraficaId);
    }
}