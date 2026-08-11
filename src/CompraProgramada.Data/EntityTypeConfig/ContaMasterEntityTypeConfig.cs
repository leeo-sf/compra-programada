using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class ContaMasterEntityTypeConfig : EntityBaseContaConfiguration<ContaMaster>
{
    public override void Configure(EntityTypeBuilder<ContaMaster> builder)
    {
        base.Configure(builder);

        builder.ToTable("conta_master");
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasComment("tipo da conta");
        builder.HasMany(x => x.CustodiaMasters).WithOne(x => x.ContaMaster).HasForeignKey(x => x.ContaMasterId);
    }
}