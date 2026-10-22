# Catálogo de Cursos — Case 519/2026

Projeto demonstrativo e não oficial, baseado nos requisitos do desafio SESI/SENAI-SP. **Dados fictícios.**

## Tecnologias
- ASP.NET Core 10, Entity Framework Core 10, Npgsql / PostgreSQL 17
- React 19, TypeScript, Vite
- Docker Compose (somente banco)

## Arquitetura
`React -> REST API ASP.NET Core -> EF Core -> PostgreSQL`.
Entidades: `Categoria` 1:N `Curso` 1:N `Modulo` 1:N `Aula`.

## Execução local (PowerShell)
Pré-requisitos: .NET SDK 10, Docker Desktop (Docker Compose), Node.js compatível com Vite, npm.

```powershell
cd C:\Projetos\sesi-catalogo-cursos
docker compose up -d postgres
cd .\backend\CatalogoCursos.Api
dotnet restore
dotnet tool install --global dotnet-ef --version 10.0.0  # execute apenas se ainda não tiver instalado
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run --urls http://localhost:5080
```
Em outro PowerShell:
```powershell
cd C:\Projetos\sesi-catalogo-cursos\frontend
Copy-Item .env.example .env
npm install
npm run dev
```
Abra http://localhost:5173 e http://localhost:5080/swagger .

**Observação:** `dotnet ef migrations add` gera os arquivos de migration, que devem ser commitados. Na execução de outro computador, rode somente `dotnet ef database update`. O seeder insere dados fictícios no primeiro início em Development após criar o banco.

## API
- `GET /health` — saúde do processo (não verifica o banco)
- `GET /api/cursos` — lista `{total,itens}`
- `GET /api/cursos?pesquisa=matematica&categoria=Matemática&modalidade=Online&status=Disponível` — combina pesquisa e filtros
- `GET /api/cursos/{id}` — detalhe, módulos e aulas
- `GET /api/filtros` — categorias, modalidades e status

## Responsividade e acessibilidade
Estrutura responsiva com breakpoints 900px/600px; labels vinculados a inputs; foco visível; regiões semânticas, avisos com `role=alert` e contagem com `aria-live`. Contraste e correspondência exata ao Figma ainda exigem verificação.

## Publicação
A definir após testes locais; produção exige string de conexão via variável de ambiente `ConnectionStrings__DefaultConnection`, execução controlada de migrations e configuração restrita de CORS com origem do frontend. Nunca publicar credenciais reais.

## Limitações e pendências
- Base inicial preparada, **não validada em build .NET neste ambiente**.
- Visual é provisório: necessita conferência quadro a quadro com o protótipo Figma.
- Os dados são fictícios próprios e deverão ser reconciliados com os dados apresentados no Figma.
- Não há autenticação (não requisitada), nem integração Moodle (diferencial opcional).
- Publicação, testes automatizados, migração versionada e ZIP final de entrega ainda pendentes.
- Configurações locais de banco são somente para desenvolvimento; revisar antes de produção.
