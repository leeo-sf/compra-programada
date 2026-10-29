using CompraProgramada.Domain.Entity.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class EntityBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : EntidadeBase
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityByDefaultColumn().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired().HasComment("data de criação");
        builder.Property(x => x.DataAtualizacao).HasColumnName("data_atualizacao").IsRequired().HasComment("data de atualização");
    }
}