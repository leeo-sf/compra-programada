using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CustodiaMasterEntityTypeConfig : IEntityTypeConfiguration<CustodiaMaster>
{
    public void Configure(EntityTypeBuilder<CustodiaMaster> builder)
    {
        builder.ToTable("custodia_master");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.ContaMasterId).IsRequired().HasColumnName("conta_master_id").HasComment("identificador conta");
        builder.Property(x => x.Ticker).HasColumnName("ticker").HasComment("ativo");
        builder.Property(x => x.QuantidadeResiduo).HasColumnName("quantidade_residuo").HasDefaultValue(0).HasComment("quantidade de ativos que sobraram");
        builder.Property(x => x.ConsideradoNovaCompra).HasColumnName("considerado_nova_compra").HasDefaultValue(false).HasComment("status se já foi considerado na nova compra");
    }
}