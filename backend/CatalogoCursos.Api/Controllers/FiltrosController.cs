using CatalogoCursos.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CatalogoCursos.Api.Controllers;
[ApiController]
[Route("api/filtros")]
public class FiltrosController(CatalogoDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var categorias = await db.Categorias.AsNoTracking().OrderBy(c => c.Nome).Select(c => c.Nome).ToListAsync(ct);
        var modalidades = await db.Cursos.AsNoTracking().Select(c => c.Modalidade).Distinct().OrderBy(x => x).ToListAsync(ct);
        var status = await db.Cursos.AsNoTracking().Select(c => c.Status).Distinct().OrderBy(x => x).ToListAsync(ct);
        return Ok(new { categorias, modalidades, status });
    }
}
