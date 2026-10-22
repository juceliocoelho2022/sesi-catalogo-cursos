namespace CatalogoCursos.Api.DTOs;
public record CursoResumoDto(int Id, string NomeCompleto, string NomeBreve, string Descricao,
    string Categoria, string Modalidade, int CargaHoraria, string PublicoAlvo, string Status,
    string ImagemUrl, DateOnly? DataDisponibilizacao);
public record AulaDto(int Id, string Titulo, string Descricao, int Ordem);
public record ModuloDto(int Id, string Titulo, int Ordem, List<AulaDto> Aulas);
public record CursoDetalheDto(int Id, string NomeCompleto, string NomeBreve, string Descricao,
    string Categoria, string Modalidade, int CargaHoraria, string PublicoAlvo, string Status,
    string ImagemUrl, DateOnly? DataDisponibilizacao, List<ModuloDto> Modulos);
public record CursosRespostaDto(int Total, IReadOnlyList<CursoResumoDto> Itens);
