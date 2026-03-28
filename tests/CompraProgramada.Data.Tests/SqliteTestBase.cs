using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Tests;

public class SqliteTestBase : IDisposable
{
    protected readonly SqliteConnection _connection;
    protected readonly AppDbContext _context;

    protected SqliteTestBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Close();
    }
}