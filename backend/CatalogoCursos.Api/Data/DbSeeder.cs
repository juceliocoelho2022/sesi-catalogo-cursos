using CatalogoCursos.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogoCursos.Api.Data;
public static class DbSeeder
{
    public static async Task SeedAsync(CatalogoDbContext db)
    {
        if (await db.Cursos.AnyAsync()) return;
        var matematica = new Categoria { Nome = "Matemática" };
        var linguagens = new Categoria { Nome = "Linguagens" };
        var natureza = new Categoria { Nome = "Ciências da Natureza" };
        db.Categorias.AddRange(matematica, linguagens, natureza);
        var cursos = new (string Nome, string Breve, string Desc, Categoria Categoria, string Modalidade, int Horas, string Publico, string Status, string Imagem, DateOnly? Data)[] {
            ("Matemática Aplicada ao Cotidiano", "Matemática Aplicada", "Raciocínio quantitativo em problemas reais.", matematica, "Online", 20, "Estudantes do ensino médio", "Disponível", "/assets/hero-image.png", new DateOnly(2026, 10, 1)),
            ("Leitura e Produção de Texto", "Produção de Texto", "Estratégias de compreensão e produção escrita.", linguagens, "Online", 30, "Jovens e adultos", "Disponível", "/assets/banner-ava.png", new DateOnly(2026, 9, 15)),
            ("Introdução à Sustentabilidade", "Sustentabilidade", "Consumo consciente e cidadania ambiental.", natureza, "Presencial", 16, "Comunidade escolar", "Em breve", "/assets/video-thumb.png", new DateOnly(2026, 11, 10)),
            ("Laboratório de Ciências Experimentais", "Laboratório de Ciências", "Práticas introdutórias de investigação científica.", natureza, "Presencial", 40, "Estudantes do ensino fundamental", "Disponível", "/assets/hero-image.png", new DateOnly(2026, 8, 20)),
            ("Geometria e Visualização Espacial", "Geometria", "Estudo de formas, medidas e espaço.", matematica, "Online", 24, "Professores e estudantes", "Encerrado", "/assets/banner-ava.png", new DateOnly(2026, 6, 1)),
            ("Comunicação e Linguagem Digital", "Linguagem Digital", "Comunicação responsável em ambientes digitais.", linguagens, "Presencial", 12, "Profissionais da educação", "Em breve", "/assets/video-thumb.png", new DateOnly(2026, 12, 1))
        };
        foreach (var c in cursos)
        {
            var curso = new Curso { NomeCompleto=c.Nome, NomeBreve=c.Breve, Descricao=c.Desc,
                Categoria=c.Categoria, Modalidade=c.Modalidade, CargaHoraria=c.Horas,
                PublicoAlvo=c.Publico, Status=c.Status, ImagemUrl=c.Imagem, DataDisponibilizacao=c.Data };
            curso.Modulos.Add(new Modulo { Titulo = "Fundamentos", Ordem = 1, Aulas = [
                new Aula { Titulo = "Introdução", Descricao = $"Apresentação dos conceitos essenciais de {c.Breve}.", Ordem = 1 },
                new Aula { Titulo = "Aplicações práticas", Descricao = $"Atividades orientadas e situações-problema de {c.Breve}.", Ordem = 2 }
            ] });
            curso.Modulos.Add(new Modulo { Titulo = "Desenvolvimento", Ordem = 2, Aulas = [
                new Aula { Titulo = "Projeto de aprendizagem", Descricao = $"Elaboração de uma atividade aplicada relacionada a {c.Breve}.", Ordem = 1 }
            ] });
            db.Cursos.Add(curso);
        }
        await db.SaveChangesAsync();
    }
}
