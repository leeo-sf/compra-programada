using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraProgramada.Data.EntityTypeConfig;

internal class ComposicaoCotacaoEntityTypeConfig : IEntityTypeConfiguration<ComposicaoCotacao>
{
    public void Configure(EntityTypeBuilder<ComposicaoCotacao> builder)
    {
        builder.ToTable("composicao_cotacao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id").HasComment("identificador");
        builder.Property(x => x.CotacaoId).HasColumnName("cotacao_id").IsRequired().HasComment("identificador composição de cotação");
        builder.Property(x => x.Ticker).HasColumnName("ticker").IsRequired().HasComment("identificação ativo");
        builder.Property(x => x.PrecoFechamento).HasColumnName("preco_fechamento").IsRequired().HasComment("preço do fechamento do ativo");
    }
}