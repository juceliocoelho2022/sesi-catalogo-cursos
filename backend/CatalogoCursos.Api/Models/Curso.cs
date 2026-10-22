namespace CatalogoCursos.Api.Models;
public class Curso
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string NomeBreve { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public string Modalidade { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public string PublicoAlvo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ImagemUrl { get; set; } = string.Empty;
    public DateOnly? DataDisponibilizacao { get; set; }
    public List<Modulo> Modulos { get; set; } = [];
}
