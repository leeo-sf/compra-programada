using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Cliente { get; set; } = default!;
    public DbSet<ContaGrafica> ContaGrafica { get; set; } = default!;
    public DbSet<ContaMaster> ContaMaster { get; set; } = default!;
    public DbSet<CestaRecomendada> CestaRecomendada { get; set; } = default!;
    public DbSet<ComposicaoCesta> ComposicaoCesta { get; set; } = default!;
    public DbSet<HistoricoExecucaoMotor> HistoricoExecucaoMotor { get; set; } = default!;
    public DbSet<Cotacao> Cotacao { get; set; } = default!;
    public DbSet<ComposicaoCotacao> ComposicaoCotacao { get; set; } = default!;
    public DbSet<CustodiaMaster> CustodiaMaster { get; set; } = default!;
    public DbSet<OrdemCompra> OrdemCompra { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}