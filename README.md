# Catálogo de Cursos — Desafio Técnico 519/2026

> **Projeto demonstrativo, não oficial**, desenvolvido para o processo seletivo de **Desenvolvedor de Sistemas I — SESI/SENAI-SP**. Os dados de cursos são fictícios; nomes e arquivos visuais fornecidos no desafio são utilizados exclusivamente como referência do case.

## 1. Visão geral

O Centro de Tecnologias e Educação Digital (CTED) necessita de um catálogo web para consultar ofertas de cursos de um ambiente educacional. A solução permite pesquisar pelo nome, combinar filtros por categoria, modalidade e status, consultar a quantidade de resultados e visualizar módulos e aulas de cada curso.

**Escopo intencional:** catálogo **somente leitura**, sem cadastro, edição ou exclusão. A integração com Moodle não é necessária ao atendimento dos requisitos mínimos e não está implementada.

**Documentos de apoio**
- [Regras de negócio e critérios de aceitação](docs/REGRAS_DE_NEGOCIO.md)
- [Roteiro para apresentação técnica](docs/ROTEIRO_DEMONSTRACAO.md)
- [Registro do uso de IA](USO_DE_IA.md)
- [Auditoria de aderência ao edital 519/2026](docs/AUDITORIA_CONFORMIDADE_519_2026.md)
- [Comparação com o Figma e diferenças justificadas](docs/DIFERENCAS_FIGMA.md)

### Situação e evidências

| Item | Situação |
| --- | --- |
| ASP.NET Core compilado no Windows com .NET SDK 10.0.402 | **Validado pelo candidato** |
| `GET /health` retorna `{"status":"ok"}` | **Validado pelo candidato** |
| `GET /api/cursos` retorna seis cursos | **Validado pelo candidato** |
| Filtros isolados de Matemática e Linguagens retornam dois cursos cada | **Validado pelo candidato** |
| Frontend React exibindo os cursos | **Observado em captura de tela** |
| Exibição de detalhes dentro do card e chips de filtros | **Validado visualmente no Windows pelo candidato** |
| Figma e acessibilidade | **Protótipo comparado em modo apresentação; landing alinhada aos elementos principais; auditoria mobile/WCAG pendente** |
| Testes automatizados | **12 cenários de API + PostgreSQL validados no GitHub Actions**; UI mobile e a11y ainda pendentes |
| Aplicação publicada em hospedagem gratuita | **Live no Render; URL pública e API consultadas em 22/10/2026** |

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

**Automação:** execute `bash tests/smoke-api.sh http://127.0.0.1:5080` em um terminal Bash com `curl` e `jq`, após iniciar API e PostgreSQL com dados fictícios. O GitHub Actions executa os mesmos 12 casos com um PostgreSQL 17 real e disponibiliza o ZIP final no job `entrega-zip`.

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

## 10. Publicação gratuita — Render

**A aplicação está publicada e acessível publicamente.**

- **Interface web:** <https://sesi-catalogo-519-web.onrender.com/>
- **Health check:** <https://sesi-catalogo-519-web.onrender.com/health>
- **Catálogo API:** <https://sesi-catalogo-519-web.onrender.com/api/cursos>
- **Detalhes API:** <https://sesi-catalogo-519-web.onrender.com/api/cursos/1>
- **Opções de filtros:** <https://sesi-catalogo-519-web.onrender.com/api/filtros>

**Infraestrutura implantada em 22/10/2026:**
- Render Web Service **Free** com Docker; React e ASP.NET Core 10 na mesma origem HTTPS.
- Render PostgreSQL 17 **Free**, banco `sesi-catalogo-519-db` exclusivo do case.
- Branch de implantação atual: `feature/filtros-ux`; a `main` ainda está preservada aguardando validação e merge.
- A conexão remota é fornecida como `DATABASE_URL` por um grupo de variáveis do Render, sem incluir segredo no GitHub.
- `SEED_DEMO_DATA=true` permite aplicar a migration versionada e inserir os seis cursos de demonstração somente se o banco estiver vazio.

O `Dockerfile` e o `render.yaml` versionados documentam a infraestrutura. A criação efetiva dos dois serviços foi feita diretamente na conta Render, com o repositório GitHub vinculado.

### Evidências da validação pública (22/10/2026)

| Cenário | Resultado observado |
| --- | --- |
| Render Web Service | **Live**, commit `a2111d1` |
| `GET /health` | `{"status":"ok"}` |
| `GET /api/cursos` | `total: 6` |
| `GET /api/filtros` | 3 categorias, 2 modalidades, 3 status |
| `GET /api/cursos?categoria=Linguagens` | 2 cursos |
| `GET /api/cursos?categoria=Matemática` | 2 cursos |
| `GET /api/cursos?pesquisa=Matemática&categoria=Linguagens` | 0 cursos, conforme filtro combinado |
| `GET /api/cursos/1` | Detalhes com 2 módulos |
| Página inicial | Frontend React renderizado com dados dos cursos |

**Ainda requer validação manual:** aferição pixel a pixel com o Figma, navegação por teclado e responsividade mobile, imagens individuais na hospedagem, comportamento de abertura/fechamento do detalhe em navegador externo e acessibilidade. O endpoint `/health` comprova disponibilidade do processo, não saúde do banco; os endpoints de cursos confirmam também a consulta real.

### Verificação por PowerShell

```powershell
$site = "https://sesi-catalogo-519-web.onrender.com"
Invoke-RestMethod "$site/health"
Invoke-RestMethod "$site/api/cursos"
Invoke-RestMethod "$site/api/cursos?categoria=Linguagens"
Invoke-RestMethod "$site/api/cursos/1" | ConvertTo-Json -Depth 10
```

**Artefato compilado:** no [GitHub Actions](https://github.com/juceliocoelho2022/sesi-catalogo-cursos/actions/runs/39774064375), abra o job `Artefato - ZIP de publicacao` e baixe `sesi-519-publicacao`. Ele contém `sesi-519-publicacao.zip` com a aplicação publicada, documentação e um `codigo-fonte.zip`. O arquivo ainda precisa ser inspecionado antes de anexá-lo à entrega.

**Limitações do plano Free:** a instância web pode hibernar após 15 minutos de inatividade; o primeiro acesso pode demorar. O PostgreSQL Free foi criado em **22/10/2026** com expiração indicada pelo Render em **21/11/2026**. Isso não afeta a entrega imediata, mas restringe a disponibilidade de longo prazo.

Para recriar a infraestrutura e revisar os cuidados de segurança, consulte [o guia de publicação](docs/PUBLICACAO_RENDER.md).


### Adequação visual ao protótipo Figma

Uma inspeção do protótipo [TEST DEV — SESI Indústria] identificou as telas **Landing Page**, **Detalhes Curso** e **AVA**. O frontend recebeu uma landing com hero vermelho e imagem industrial, título e CTA `Conhecer cursos`, além de cabeçalho navegável. O catálogo com filtros continua funcional na mesma página. Autenticação, matrícula e AVA não foram simulados porque o escopo mínimo da atividade exige consulta, não uma plataforma educacional completa.

**Atenção:** a tela de detalhes na nossa implementação é expandida dentro do card, em vez de uma página independente. Os seis cursos fictícios também diferem do curso demonstrativo único do Figma. Essas e outras escolhas estão justificadas em [docs/DIFERENCAS_FIGMA.md](docs/DIFERENCAS_FIGMA.md). A avaliação detalhada das dimensões visuais e a validação responsiva permanecem pendentes.

## 11. Limitações conhecidas e decisões de escopo

- O Figma foi inspecionado em modo protótipo e as diferenças de escopo/identidade foram registradas em [docs/DIFERENCAS_FIGMA.md](docs/DIFERENCAS_FIGMA.md). Medidas pixel a pixel e validação mobile ainda estão pendentes.
- MVP somente leitura, sem autenticação, matrícula, inscrição, pagamento, CRUD ou Moodle.
- Os 12 cenários automatizados de API estão implementados e passaram no CI; falta automação E2E da interface e a revisão de acessibilidade/mobile.
- Sem cache, paginação, telemetria de produção ou regras rígidas de validação para status/modalidade.
- Dados e imagens são fictícios/de demonstração; o material visual pertence ao desafio.
- Infraestrutura Render gratuita publicada e API validada em 22/10/2026; arquivo ZIP de publicação gerado no GitHub Actions, aguardando inspeção/download pelo candidato.
- O `health` não avalia a saúde do PostgreSQL.
- Os controles de acessibilidade foram iniciados, sem auditoria formal WCAG.

## 12. Uso de IA e autoria

A IA foi empregada como apoio à leitura do desafio, estruturação do projeto e sugestões de implementação e documentação. O candidato deve validar código, decidir o que aceitar ou descartar e registrar um erro real corrigido no arquivo [USO_DE_IA.md](USO_DE_IA.md). Não atribuir à IA resultados de execução que não foram obtidos.

---

**Repositório:** [juceliocoelho2022/sesi-catalogo-cursos](https://github.com/juceliocoelho2022/sesi-catalogo-cursos)  
**Processo:** Desenvolvedor de Sistemas I — 519/2026  
**Entrega prevista no edital:** 26/10/2026 às 08h (horário de Brasília).
