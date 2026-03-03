using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class HistoricoExecucaoMotorEntityTypeConfig : IEntityTypeConfiguration<HistoricoExecucaoMotor>
{
    public void Configure(EntityTypeBuilder<HistoricoExecucaoMotor> builder)
    {
        builder.ToTable("historico_execucao_motor");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.DataReferencia).IsRequired().HasColumnName("data_referencia").HasComment("data que deveria ser executada");
        builder.Property(x => x.DataExecucao).IsRequired().HasColumnName("data_execucao").HasComment("data que foi executado");
        builder.Property(x => x.Executado).HasColumnName("executado").IsRequired().HasDefaultValue(true).HasComment("compra feita");
    }
}