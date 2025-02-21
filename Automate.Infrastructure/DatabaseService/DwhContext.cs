using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Automate.Infrastructure.DatabaseService;

public class DwhContext<T>(string connectionStr) : DbContext where T : class
{
#pragma warning disable CS8618 // CS8618 Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal DbSet<T> Result { get; set; }
#pragma warning restore CS8618 // CS8618 Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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