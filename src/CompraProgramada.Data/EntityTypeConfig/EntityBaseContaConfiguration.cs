using CompraProgramada.Domain.Entity.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class EntityBaseContaConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseConta
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.NumeroConta).HasColumnName("numero_conta").HasComment("número da conta");
        builder.Property(x => x.DataCriacao).HasColumnName("data_criacao");
    }
}