using CatalogoCursos.Api.Data;
using CatalogoCursos.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CatalogoCursos.Api.Controllers;
[ApiController]
[Route("api/cursos")]
public class CursosController(CatalogoDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CursosRespostaDto>> Listar(
        [FromQuery] string? pesquisa, [FromQuery] string? categoria,
        [FromQuery] string? modalidade, [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var query = db.Cursos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var busca = $"%{pesquisa.Trim()}%";
            query = query.Where(c => EF.Functions.ILike(c.NomeCompleto, busca) || EF.Functions.ILike(c.NomeBreve, busca));
        }
        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(c => c.Categoria.Nome == categoria.Trim());
        if (!string.IsNullOrWhiteSpace(modalidade))
            query = query.Where(c => c.Modalidade == modalidade.Trim());
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Status == status.Trim());
        var itens = await query.OrderBy(c => c.NomeCompleto).Select(c => new CursoResumoDto(
            c.Id, c.NomeCompleto, c.NomeBreve, c.Descricao, c.Categoria.Nome,
            c.Modalidade, c.CargaHoraria, c.PublicoAlvo, c.Status,
            c.ImagemUrl, c.DataDisponibilizacao)).ToListAsync(cancellationToken);
        return Ok(new CursosRespostaDto(itens.Count, itens));
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CursoDetalheDto>> Detalhe(int id, CancellationToken cancellationToken)
    {
        var curso = await db.Cursos.AsNoTracking().Where(c => c.Id == id).Select(c => new CursoDetalheDto(
            c.Id, c.NomeCompleto, c.NomeBreve, c.Descricao, c.Categoria.Nome,
            c.Modalidade, c.CargaHoraria, c.PublicoAlvo, c.Status, c.ImagemUrl,
            c.DataDisponibilizacao, c.Modulos.OrderBy(m => m.Ordem).Select(m => new ModuloDto(
                m.Id, m.Titulo, m.Ordem, m.Aulas.OrderBy(a => a.Ordem)
                  .Select(a => new AulaDto(a.Id, a.Titulo, a.Descricao, a.Ordem)).ToList()
            )).ToList())).FirstOrDefaultAsync(cancellationToken);
        return curso is null ? NotFound(new { mensagem = "Curso não encontrado." }) : Ok(curso);
    }
}
