using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class ContaMasterEntityTypeConfig : IEntityTypeConfiguration<ContaMaster>
{
    public void Configure(EntityTypeBuilder<ContaMaster> builder)
    {
        builder.ToTable("conta_master");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.NumeroConta).HasColumnName("numero_conta").HasComment("número conta");
        builder.Property(x => x.DataCriacao).HasColumnName("data_criacao");
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasComment("tipo conta");
        builder.HasMany(x => x.CustodiaMasters).WithOne(x => x.ContaMaster).HasForeignKey(x => x.ContaMasterId);
    }
}