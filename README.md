# Catálogo de Cursos — Desafio Técnico 519/2026

> **Projeto demonstrativo, não oficial**, desenvolvido para o processo seletivo de **Desenvolvedor de Sistemas I — SESI/SENAI-SP**. Os dados de cursos são fictícios; nomes e arquivos visuais fornecidos no desafio são utilizados exclusivamente como referência do case.

## 1. Visão geral

O Centro de Tecnologias e Educação Digital (CTED) necessita de um catálogo web para consultar ofertas de cursos de um ambiente educacional. A solução permite pesquisar pelo nome, combinar filtros por categoria, modalidade e status, consultar a quantidade de resultados e visualizar módulos e aulas de cada curso.

**Escopo intencional:** catálogo **somente leitura**, sem cadastro, edição ou exclusão. A integração com Moodle não é necessária ao atendimento dos requisitos mínimos e não está implementada.

**Documentos de apoio**
- [Regras de negócio e critérios de aceitação](docs/REGRAS_DE_NEGOCIO.md)
- [Roteiro para apresentação técnica](docs/ROTEIRO_DEMONSTRACAO.md)
- [Registro do uso de IA](USO_DE_IA.md)

### Situação e evidências

| Item | Situação |
| --- | --- |
| ASP.NET Core compilado no Windows com .NET SDK 10.0.402 | **Validado pelo candidato** |
| `GET /health` retorna `{"status":"ok"}` | **Validado pelo candidato** |
| `GET /api/cursos` retorna seis cursos | **Validado pelo candidato** |
| Filtros isolados de Matemática e Linguagens retornam dois cursos cada | **Validado pelo candidato** |
| Frontend React exibindo os cursos | **Observado em captura de tela** |
| Exibição de detalhes dentro do card e chips de filtros | **Implementado nesta branch; teste no Windows pendente** |
| Validação abrangente de acessibilidade e fidelidade ao Figma | **Pendente** |
| Testes automatizados | **Ainda não implementados** |
| Aplicação publicada em hospedagem gratuita | **Pendente** |

Esses estados devem ser atualizados após cada evidência de teste; não se deve confundir compilação, integração em desenvolvimento e publicação em produção.

## 2. Tecnologias

| Camada | Tecnologia | Papel |
| --- | --- | --- |
| Frontend | React 19, TypeScript, Vite 7 | Interface do catálogo e consumo HTTP da API |
| Backend | C# / ASP.NET Core 10 | Endpoints REST e organização das consultas |
| Persistência | EF Core 10 + Npgsql | Mapeamento objeto-relacional e acesso ao PostgreSQL |
| Banco de dados | PostgreSQL 17 | Armazenamento de categorias, cursos, módulos e aulas |
| Ambiente local | Docker Compose | Inicialização do PostgreSQL |
| Documentação da API | Swashbuckle/Swagger (Development) | Exploração dos endpoints |
| Versionamento | Git e GitHub | Histórico, branches e revisão |

**Decisão de arquitetura:** uma aplicação pequena não exige microsserviços. Um backend ASP.NET Core com controllers, DTOs, DbContext e entidades relacionais simplifica instalação e manutenção. São usadas consultas `AsNoTracking()` porque os endpoints são somente de leitura.

## 3. Arquitetura e fluxo dos dados

```text
Navegador / React + TypeScript (porta 5173)
              |
              | HTTP / JSON
              v
ASP.NET Core Web API (porta 5080)
   |-- GET /api/cursos
   |-- GET /api/cursos/{id}
   |-- GET /api/filtros
   |-- GET /health
              |
              v
Entity Framework Core / Npgsql
              |
              v
PostgreSQL 17 em Docker (porta 5433 no host)
```

Fluxo de consulta:
1. O React mantém os valores de pesquisa e filtros em estado local.
2. Quando o usuário altera algum critério, a interface monta parâmetros de consulta.
3. A API aplica condições de pesquisa e filtros antes de executar a consulta SQL.
4. O EF Core projeta os resultados para DTOs, sem expor diretamente entidades de persistência.
5. O React atualiza a contagem, os cards ou as mensagens de carregamento/erro/ausência de resultados.
6. Ao clicar em **Ver detalhes**, a interface chama `GET /api/cursos/{id}` e expande o painel **dentro do card escolhido**. Um único painel permanece aberto por vez.

## 4. Organização das pastas

```text
sesi-catalogo-cursos/
├── backend/
│   └── CatalogoCursos.Api/
│       ├── Controllers/             # CursosController e FiltrosController
│       ├── Data/                    # CatalogoDbContext e DbSeeder
│       ├── DTOs/                    # Contratos da API
│       ├── Migrations/              # Migration inicial versionada
│       ├── Models/                  # Curso, Categoria, Modulo, Aula
│       ├── Program.cs               # DI, CORS, Swagger, endpoints, seed
│       └── appsettings.json         # Configuração local demonstrativa
├── frontend/
│   ├── public/assets/              # Imagens e ícones de referência fornecidos
│   ├── src/App.tsx                  # Catálogo, filtros, resultados e detalhes
│   ├── src/style.css                # Layout responsivo e estados de interface
│   ├── .env.example                 # URL da API
│   └── package.json
├── docs/
│   ├── REGRAS_DE_NEGOCIO.md
│   └── ROTEIRO_DEMONSTRACAO.md
├── docker-compose.yml
├── USO_DE_IA.md
└── README.md
```

## 5. Modelo de dados

```text
Categoria (1) ─── (N) Curso (1) ─── (N) Modulo (1) ─── (N) Aula
```

| Entidade | Campos importantes | Observações |
| --- | --- | --- |
| Categoria | Id, Nome | Índice único em `Nome` |
| Curso | Id, NomeCompleto, NomeBreve, Descricao, CategoriaId, Modalidade, CargaHoraria, PublicoAlvo, Status, ImagemUrl, DataDisponibilizacao | Índice único em `NomeBreve`; `DataDisponibilizacao` opcional |
| Modulo | Id, CursoId, Titulo, Ordem | Vínculo com curso; ordenação por `Ordem` nos detalhes |
| Aula | Id, ModuloId, Titulo, Descricao, Ordem | Vínculo com módulo; ordenação por `Ordem` nos detalhes |

**Integridade no banco:** chaves primárias e estrangeiras; índices únicos indicados; exclusão em cascata definida na migration. Não existem endpoints públicos de exclusão. As modalidades e os status são valores textuais da base de demonstração, **sem enum ou CHECK constraint no banco na versão atual**; não alegar validação de domínio que ainda não foi implementada.

**Dados iniciais:** o `DbSeeder` cria 3 categorias e 6 cursos fictícios, cada um com 2 módulos e 3 aulas, apenas em ambiente `Development` e apenas quando não houver cursos na tabela. Carga horária de 12 a 40 horas; modalidades Online e Presencial; status Disponível, Em breve e Encerrado. Esses dados não são extraídos do Moodle.

## 6. Execução local no Windows (PowerShell)

### Pré-requisitos
- .NET SDK **10** (verifique `dotnet --version`);
- Node.js compatível com Vite 7 e npm;
- Docker Desktop com Docker Compose;
- Git.

Ajuste o caminho abaixo se o projeto estiver em outro local:

```powershell
$raiz = "E:\sesi-catalogo-cursos-etapa1\sesi-catalogo-cursos"
Set-Location $raiz
```

### 6.1 Iniciar o PostgreSQL

```powershell
docker compose up -d postgres
docker compose ps
docker compose exec postgres psql -U sesi -d sesi_catalogo -c "SELECT current_database();"
```

O compose expõe o banco na porta **5433** do Windows, encaminhada à porta 5432 do container. O banco é `sesi_catalogo`, usuário `sesi`. **A senha do Compose é somente para demonstração local; não reutilizar em produção.**

### 6.2 Backend e migrations

```powershell
Set-Location "$raiz\backend\CatalogoCursos.Api"
dotnet restore
dotnet build
dotnet ef database update
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --urls http://localhost:5080
```

**Importante:** a migration `InitialCreate` já está versionada em `Migrations/`. Em outra instalação, use **somente `dotnet ef database update`** para aplicá-la; não execute `dotnet ef migrations add InitialCreate` outra vez.

Se `dotnet ef` não existir, instale ou atualize a ferramenta compatível com EF Core 10:
```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
```

Caso já exista, use `dotnet tool update --global dotnet-ef --version 10.0.13`.

O Swagger é disponibilizado em <http://localhost:5080/swagger> no ambiente `Development`.

### 6.3 Frontend

Em **outro terminal**, mantendo a API em execução:

```powershell
Set-Location "$raiz\frontend"
if (!(Test-Path .env)) { Copy-Item .env.example .env }
npm ci
npm run build
npm run dev
```

Abra <http://localhost:5173>. A variável `VITE_API_URL` do `.env` aponta inicialmente para `http://localhost:5080`. `npm ci` utiliza o `package-lock.json` versionado.

## 7. Contrato HTTP

| Método / endpoint | Parâmetros | Resposta |
| --- | --- | --- |
| `GET /health` | Nenhum | `{"status":"ok"}` — verifica o processo, **não** a conectividade do banco |
| `GET /api/cursos` | `pesquisa`, `categoria`, `modalidade`, `status` (opcionais) | `{ "total": 6, "itens": [...] }` sem filtros, com seed inicial |
| `GET /api/cursos/{id}` | `id` inteiro | Objeto do curso com `modulos` e `aulas`; `404` com mensagem se ausente |
| `GET /api/filtros` | Nenhum | `{ "categorias": [...], "modalidades": [...], "status": [...] }` |

**Semântica dos filtros:** pesquisa contém trecho do nome completo ou breve, com `ILIKE` (sem diferenciar maiúsculas/minúsculas); categoria, modalidade e status usam correspondência exata com o valor informado, após remoção de espaços nas extremidades; critérios diferentes são combinados com **AND**.

```powershell
$base = "http://localhost:5080"
Invoke-RestMethod "$base/health"
Invoke-RestMethod "$base/api/cursos"
Invoke-RestMethod "$base/api/cursos?categoria=Linguagens"
Invoke-RestMethod "$base/api/cursos?pesquisa=Matem%C3%A1tica"
Invoke-RestMethod "$base/api/cursos/1" | ConvertTo-Json -Depth 10
Invoke-RestMethod "$base/api/filtros"
```

Exemplo: pesquisar por **Matemática** no nome retorna um curso na base fictícia, enquanto filtrar pela categoria **Matemática** retorna dois (incluindo *Geometria e Visualização Espacial*). Essa distinção é intencional.

## 8. Interface, responsividade e acessibilidade

- Grade com 3 colunas em telas amplas; 2 abaixo de 900 px; 1 abaixo de 600 px.
- Labels visíveis nos campos, navegação por teclado e estilo de foco visível.
- Região `aria-live` informa a contagem/estado de carregamento; `role="alert"` informa erros.
- Os filtros ativos ficam visíveis e podem ser removidos individualmente.
- **Ver detalhes** expande o conteúdo **no card selecionado**, incluindo módulos e aulas; clique novamente para fechar.
- Consultas da listagem são canceladas ao trocar rapidamente filtros; consulta anterior de detalhes também é cancelada ao alternar o card.

Essas decisões constam do código, mas a conformidade WCAG e a correspondência visual ao Figma **ainda precisam de revisão manual e testes específicos**.

## 9. Verificações manuais de aceitação

1. Limpar filtros → 6 cursos.
2. Pesquisar `Matemática` pelo nome → 1 curso.
3. Categoria `Matemática`, sem pesquisa → 2 cursos.
4. Categoria `Linguagens`, sem pesquisa → 2 cursos.
5. Modalidade `Online` → 3 cursos.
6. Combinar pesquisa `Matemática` + categoria `Linguagens` → zero resultados e mensagem orientativa.
7. Abrir *Matemática Aplicada ao Cotidiano* → detalhes aparecem **no card desse curso**, não ao final de todos os cursos.
8. Fechar e abrir outro card → somente o escolhido fica expandido.
9. Desligar a API temporariamente → mensagem de falha em vez de lista silenciosamente vazia.
10. Redimensionar para 375 px, 768 px e desktop → controles utilizáveis sem rolagem horizontal indesejada.

**Atenção:** esta lista é um roteiro de validação; não significa que todos os cenários já foram aprovados.

## 10. Publicação gratuita (Render Blueprint)

A infraestrutura de demonstração está descrita em [`render.yaml`](render.yaml) e [`Dockerfile`](Dockerfile). **A publicação ainda depende da ativação pelo candidato na conta Render; não há URL pública validada neste momento.**

A imagem Docker produz React e ASP.NET Core juntos: o React é servido de `wwwroot`, a API de `/api/*` e ambos compartilham o mesmo domínio HTTPS. Isso evita CORS entre serviços na produção. O banco PostgreSQL 17 é provisionado pelo Render no plano gratuito.

Siga o [guia de publicação detalhado](docs/PUBLICACAO_RENDER.md) para vincular o repositório privado, criar uma instância **Blueprint** em Render com os recursos **Web Free** + **Postgres Free**, executar o deploy e testar o site.

**Segurança e inicialização:** o Render injeta `DATABASE_URL` sem expor credenciais no GitHub. Com `SEED_DEMO_DATA=true`, o backend aplica a migration versionada e carrega os seis cursos fictícios apenas quando a base estiver vazia. Em desenvolvimento local, continue usando a migration normal. Os dados da publicação ficam no PostgreSQL, e não no armazenamento efêmero do container.

**Limitações do plano gratuito:** serviço web pode hibernar após 15 minutos de inatividade; a primeira visita pode levar aproximadamente um minuto. O PostgreSQL Free expira após **30 dias**. A documentação oficial está em <https://render.com/docs/free>. Não selecionar instâncias pagas.

### Validação pós-publicação (somente após receber a URL real)

```powershell
$site = "https://ENDERECO-REAL-DO-RENDER.onrender.com"
Invoke-RestMethod "$site/health"
Invoke-RestMethod "$site/api/cursos"
Invoke-RestMethod "$site/api/cursos/1" | ConvertTo-Json -Depth 10
```

Também abra a URL HTTPS do site no navegador e teste a pesquisa, os filtros combinados, o detalhamento dentro de cada card, as imagens e a interface mobile.

## 11. Limitações conhecidas e decisões de escopo

- Visual e conteúdo ainda não confrontados quadro a quadro com o Figma oficial.
- MVP somente leitura, sem autenticação, matrícula, inscrição, pagamento, CRUD ou Moodle.
- Sem testes automatizados integrados na versão atual; testes manuais são necessários.
- Sem cache, paginação, telemetria de produção ou regras rígidas de validação para status/modalidade.
- Dados e imagens são fictícios/de demonstração; o material visual pertence ao desafio.
- Infraestrutura Render gratuita preparada em Blueprint/Docker, mas ainda não ativada; ZIP final e URL pública dependem de execução.
- O `health` não avalia a saúde do PostgreSQL.
- Os controles de acessibilidade foram iniciados, sem auditoria formal WCAG.

## 12. Uso de IA e autoria

A IA foi empregada como apoio à leitura do desafio, estruturação do projeto e sugestões de implementação e documentação. O candidato deve validar código, decidir o que aceitar ou descartar e registrar um erro real corrigido no arquivo [USO_DE_IA.md](USO_DE_IA.md). Não atribuir à IA resultados de execução que não foram obtidos.

---

**Repositório:** [juceliocoelho2022/sesi-catalogo-cursos](https://github.com/juceliocoelho2022/sesi-catalogo-cursos)  
**Processo:** Desenvolvedor de Sistemas I — 519/2026  
**Entrega prevista no edital:** 26/10/2026 às 08h (horário de Brasília).
