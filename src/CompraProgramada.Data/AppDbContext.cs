using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Cliente { get; set; } = default!;
    public DbSet<ContaGrafica> ContaGrafica { get; set; } = default!;
    public DbSet<CestaRecomendada> Cesta { get; set; } = default!;
    public DbSet<ComposicaoCesta> ComposicaoCesta { get; set; } = default!;
    public DbSet<HistoricoExecucaoMotor> HistoricoCompra { get; set; } = default!;
    public DbSet<Cotacao> Cotacao { get; set; } = default!;
    public DbSet<ComposicaoCotacao> ComposicaoCotacao { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}