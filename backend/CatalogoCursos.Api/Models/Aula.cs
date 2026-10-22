namespace CatalogoCursos.Api.Models;
public class Aula
{
    public int Id { get; set; }
    public int ModuloId { get; set; }
    public Modulo Modulo { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Ordem { get; set; }
}
