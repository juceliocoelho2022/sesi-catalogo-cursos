using CatalogoCursos.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogoCursos.Api.Data;
public class CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : DbContext(options)
{
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<Aula> Aulas => Set<Aula>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>().HasIndex(c => c.Nome).IsUnique();
        modelBuilder.Entity<Curso>().HasIndex(c => c.NomeBreve).IsUnique();
        modelBuilder.Entity<Curso>().HasOne(c => c.Categoria).WithMany(x => x.Cursos).HasForeignKey(c => c.CategoriaId);
        modelBuilder.Entity<Modulo>().HasOne(m => m.Curso).WithMany(c => c.Modulos).HasForeignKey(m => m.CursoId);
        modelBuilder.Entity<Aula>().HasOne(a => a.Modulo).WithMany(m => m.Aulas).HasForeignKey(a => a.ModuloId);
    }
}
