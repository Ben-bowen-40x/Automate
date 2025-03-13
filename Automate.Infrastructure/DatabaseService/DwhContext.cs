using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Automate.Infrastructure.DatabaseService;

public class DwhContext<T>(string connectionStr) : DbContext where T : class
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
    internal DbSet<T> Result { get; set; }
    internal string ConnectionString { get; set; } = connectionStr;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySQL(ConnectionString);
    }
}