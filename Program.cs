using Microsoft.EntityFrameworkCore;
using wt_lab7_postgres_efcore_liseychikov.Models;
using wt_lab7_postgres_efcore_liseychikov.Data;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var databaseUrl = builder.Configuration["DATABASE_URL"];

if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);

    connectionString = 
        $"Host={uri.Host};" +
        $"Port={(uri.Port > 0 ? uri.Port : 5432)};" +
        $"Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={Uri.UnescapeDataString(userInfo[0])};" +
        $"Password={Uri.UnescapeDataString(userInfo[1])};" +
        "SSL Mode=Disable";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/tasks", async (AppDbContext db) =>
{
    var tasks = await db.Tasks.ToListAsync();
    return Results.Ok(tasks);
});

app.MapGet("/", () => "Minimal API работает. Это добавленная строчка кода");

app.MapGet("/api/info", () =>
{
    return new
    {
        Application = "Web Technologies",
        Technology = "ASP .NET Core Minimal API",
        Version = "1.0"
    };
});

app.MapGet("/api/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
    if (task == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(task);
});

app.MapGet("/api/tasks/search", async (string? search, AppDbContext db) =>
{
    var query = db.Tasks.AsQueryable();
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
    }

    var tasks = await query.ToListAsync();
    return Results.Ok(tasks);
});

app.MapPost("/api/tasks", async (TaskModel task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.MapPut("/api/tasks/{id}", async (
    int id,
    TaskModel updatedTask,
    AppDbContext db) =>
{
    var task = await db.Tasks
        .FirstOrDefaultAsync(x => x.Id == id);

    if (task == null)
        return Results.NotFound();

    task.Name = updatedTask.Name;
    task.Category = updatedTask.Category;
    task.Price = updatedTask.Price;
    task.IsAvaliable = updatedTask.IsAvaliable;

    await db.SaveChangesAsync();

    return Results.Ok(task);
});

app.MapDelete("/api/tasks/{id}", async (
    int id, 
    AppDbContext db) =>
{
    var task = await db.Tasks
        .FirstOrDefaultAsync(x => x.Id == id);

    if (task == null)
        return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapGet("/api/tasks/filter", async (AppDbContext db) =>
{
    var tasks = await db.Tasks.Where(x => x.Price > 2000).ToListAsync();
    return Results.Ok(tasks);
});

// Исправлен путь с точки на слэш (/api/tasks/sorted)
app.MapGet("/api/tasks/sorted", async (AppDbContext db) =>
{
    var tasks = await db.Tasks.OrderBy(x => x.Price).ToListAsync();
    return Results.Ok(tasks);
});

app.MapGet("/api/tasks/avaliable", async (AppDbContext db) =>
{
    var tasks = await db.Tasks.Where(x => x.IsAvaliable == true).ToListAsync();
    return Results.Ok(tasks);
});

app.MapGet("/api/tasks/top-expensive-available", async (AppDbContext db) =>
{
    var tasks = await db.Tasks
        .Where(x => x.IsAvaliable == true)
        .OrderByDescending(x => x.Price)  
        .Take(5)                     
        .ToListAsync();

    return Results.Ok(tasks);
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");