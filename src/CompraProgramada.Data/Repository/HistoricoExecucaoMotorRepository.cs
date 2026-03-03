using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class HistoricoExecucaoMotorRepository : IHistoricoExecucaoMotorRepository
{
    private readonly AppDbContext _context;

    public HistoricoExecucaoMotorRepository(AppDbContext context) => _context = context;

    public async Task<HistoricoExecucaoMotor?> ObtemExecucaoRealizadaAsync(DateTime dataDeExecucao, CancellationToken cancellationToken)
        => await _context.HistoricoCompra
            .AsNoTracking()
            .FirstOrDefaultAsync(he => he.DataExecucao.Date == dataDeExecucao.Date && he.Executado, cancellationToken);

    public async Task CriarHistoricoExecucaoAsync(HistoricoExecucaoMotor execucao, CancellationToken cancellationToken)
    {
        _context.HistoricoCompra.Add(execucao);
        await _context.SaveChangesAsync(cancellationToken);
    }
}