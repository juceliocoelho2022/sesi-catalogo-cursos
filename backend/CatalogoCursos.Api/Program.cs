using CatalogoCursos.Api.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Local: ConnectionStrings:DefaultConnection. Render: DATABASE_URL fornecida pelo Blueprint.
// DATABASE_URL usa URI postgres://..., enquanto Npgsql espera pares chave=valor.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = builder.Configuration["DATABASE_URL"];
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var databaseUri = new Uri(databaseUrl);
    if (databaseUri.Scheme is not ("postgres" or "postgresql"))
        throw new InvalidOperationException("DATABASE_URL deve usar o esquema postgres ou postgresql.");

    var credentials = databaseUri.UserInfo.Split(':', 2);
    if (credentials.Length != 2)
        throw new InvalidOperationException("DATABASE_URL não contém usuário e senha.");

    var options = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.IsDefaultPort ? 5432 : databaseUri.Port,
        Database = Uri.UnescapeDataString(databaseUri.AbsolutePath.Trim('/')),
        Username = Uri.UnescapeDataString(credentials[0]),
        Password = Uri.UnescapeDataString(credentials[1]),
        SslMode = SslMode.Prefer
    };
    connectionString = options.ConnectionString;
}

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection ou DATABASE_URL.");

builder.Services.AddDbContext<CatalogoDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorCodesToAdd: null)));

// Permitimos localhost apenas durante o desenvolvimento com Vite.
// Em produção, o React é servido pelo próprio backend (mesma origem).
builder.Services.AddCors(options => options.AddPolicy("FrontendLocal", policy =>
    policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
          .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("FrontendLocal");
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Em Render Free não há pre-deploy command gratuito. O flag é intencional:
// aplica migrations versionadas e popula os 6 cursos na primeira inicialização.
var seedProduction = string.Equals(
    app.Configuration["SEED_DEMO_DATA"], "true", StringComparison.OrdinalIgnoreCase);

if (seedProduction || app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CatalogoDbContext>();
    if (seedProduction)
        await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

// Rotas desconhecidas do frontend recebem a página principal.
// Não substitui as rotas /api dos controllers.
app.MapFallbackToFile("index.html");
app.Run();
