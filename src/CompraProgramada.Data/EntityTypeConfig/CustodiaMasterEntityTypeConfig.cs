using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class CustodiaMasterEntityTypeConfig : EntityBaseConfiguration<CustodiaMaster>
{
    public override void Configure(EntityTypeBuilder<CustodiaMaster> builder)
    {
        base.Configure(builder);

        builder.ToTable("custodia_master");
        builder.Property(x => x.ContaMasterId).IsRequired().HasColumnName("conta_master_id").HasComment("identificador conta");
        builder.Property(x => x.Ticker).HasColumnName("ticker").HasComment("ativo");
        builder.Property(x => x.QuantidadeResiduo).HasColumnName("quantidade_residuo").HasDefaultValue(0).HasComment("quantidade de ativos que sobraram");
        builder.Property(x => x.PrecoMedio).HasColumnName("preco_medio").HasComment("preço médio dos ativos");
    }
}