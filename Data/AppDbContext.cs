using Microsoft.EntityFrameworkCore;
using wt_lab7_postgres_efcore_liseychikov.Models;

namespace wt_lab7_postgres_efcore_liseychikov.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<TaskModel> Tasks => Set<TaskModel>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<TaskModel>().HasData(
        new TaskModel
        {
            Id = 1,
            Name = "Проектирование базы данных",
            Category = "Базы данных",
            Price = 2500,
            IsAvaliable = true,
            CreatedAt = DateTime.SpecifyKind(new DateTime(2026, 1, 1), DateTimeKind.Utc)
        },
        new TaskModel
        {
            Id = 2,
            Name = "Настройка эндпоинтов API",
            Category = "Backend",
            Price = 1800,
            IsAvaliable = true,
            CreatedAt = DateTime.SpecifyKind(new DateTime(2026, 1, 2), DateTimeKind.Utc)
        }
    );
}
}