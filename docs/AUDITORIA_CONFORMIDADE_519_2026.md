# Pente-fino de conformidade — Processo Seletivo 519/2026

**Data da auditoria:** 22/10/2026  
**Documentos oficiais consultados:** `Instruções_Atividade prática_519_2026_DEV.pdf` (3 páginas), `Intruções_email_519_2026-DEV.docx` (1 página) e conteúdo do ZIP `Arquivos_Atividade prática_519_2026_DEV.zip`.  
**Código avaliado:** branch `feature/filtros-ux`, PR #1, GitHub `juceliocoelho2022/sesi-catalogo-cursos`.  
**Aplicação pública:** <https://sesi-catalogo-519-web.onrender.com/>.  
**Prazo:** 26/10/2026 às 08h. **Apresentação técnica:** entrevista presencial.

## 1. Veredicto

**Aderência funcional elevada, mas aprovação final condicionada.** Foram confirmados backend C#/.NET, frontend React, banco open source, publicação Free, API listando seis cursos e testes de categorias, busca e detalhamento. Há pendências materiais de **comparação ao Figma**, **evidências de responsividade e acessibilidade**, **pacote ZIP** e **acesso dos avaliadores ao GitHub**.

**Não declarar “100% de acordo com o Figma” até comparar o protótipo e registrar divergências.** O link consta do DOCX; o acesso técnico à estrutura e à imagem do nó `6017:342` foi recusado pela integração Figma devido à permissão insuficiente. Uma captura manual do protótipo, disponibilizada pelo candidato, permitiria executar a comparação visual.

**Link oficial do Figma:** [TEST DEV — SESI Indústria](https://www.figma.com/proto/YB0o91nnOEB5bXXCtmy5s6/-TEST-DEV--SESI-Ind%C3%BAstria?node-id=6017-342&p=f&viewport=-1570%2C411%2C0.09&t=tvoKbn8KIvr7vLak-1&scaling=contain&content-scaling=fixed&starting-point-node-id=6017%3A342&show-proto-sidebar=1&page-id=3%3A2).

## 2. Matriz de rastreabilidade do edital

Legenda: **PASSA** = evidência direta; **IMPLEMENTADO** = código existente, teste incompleto; **PENDENTE** = não comprovado/entregue; **OPCIONAL** = fora dos requisitos mínimos.

| ID | Requisito oficial | Resultado / evidência | Estado |
| --- | --- | --- | --- |
| RF01 | Catálogo web de cursos | Página pública Render carrega React com cursos | PASSA |
| RF02 | Frontend baseado no Figma oficial | Existe frontend funcional, mas não foi possível inspecionar o Figma para comparar | PENDENTE |
| RF03 | Responsividade | CSS em 900/600 px; captura desktop conferida; teste real a 375 e 768 px ainda não evidenciado | IMPLEMENTADO |
| RF04 | Acessibilidade | Labels, foco visível, `aria-live`, alertas e `aria-expanded`; sem teste WCAG/leitor de tela | IMPLEMENTADO |
| RF05 | Backend em C#/.NET | ASP.NET Core 10 e controllers com DTOs | PASSA |
| RF06 | API de consulta | `GET /api/cursos`, `GET /api/cursos/{id}`, `GET /api/filtros` | PASSA |
| RF07 | Banco de código aberto | PostgreSQL 17 em Render Free, EF Core e migrations | PASSA |
| RF08 | Integração frontend/API/banco | Página pública e API respondem; seis registros | PASSA |
| RF09 | Aplicação publicada gratuitamente | Render Web Free + Postgres Free; URL real validada | PASSA |
| RF10 | Nome completo, breve, descrição, categoria, modalidade, carga, público, status, imagem, data | Campos presentes nas entidades/DTOs; seis cursos retornados | PASSA |
| RF11 | Mínimo de 6 cursos | API retorna `total=6` | PASSA |
| RF12 | Mínimo de 3 categorias, 2 modalidades, 3 status | API de filtros retorna 3/2/3 | PASSA |
| RF13 | Diferentes cargas, módulos, descrições de aulas e públicos | Cargas/públicos variam; cursos têm módulos/aulas, mas os títulos de módulos são repetidos em toda a base | IMPLEMENTADO, REFINAR |
| RF14 | Dados fictícios | `DbSeeder.cs` define cursos fictícios; equivalência ao conteúdo específico do Figma não verificada | IMPLEMENTADO |
| RF15 | Listagem dos cursos por API | Validado publicamente | PASSA |
| RF16 | Pesquisa por nome | Retorna 1 curso para `Matemática` na base fictícia | PASSA |
| RF17 | Filtro de categoria | Matemática e Linguagens retornam 2 cada | PASSA |
| RF18 | Filtros de modalidade e status | Validados no CI de integração contra PostgreSQL; 12 testes HTTP aprovados | PASSA |
| RF19 | Pesquisa + filtros combinados | `Matemática` + `Linguagens` retorna 0 | PASSA |
| RF20 | Contagem e limpar filtros | Contagem da API visível; limpeza implementada no React | IMPLEMENTADO |
| RF21 | Abrir detalhes do curso | API e captura do candidato confirmam expansão no card | PASSA |
| RF22 | Mensagem para zero resultados | Código exibe mensagem orientativa; API retorna lista vazia | IMPLEMENTADO |
| RF23 | Estado de carregamento | Estado `carregando` e mensagem presentes | IMPLEMENTADO |
| RF24 | Mensagem de erro | Estados de erro da API e carregamento de detalhe presentes; comportamento em falha ainda precisa teste de navegador | IMPLEMENTADO |
| RF25 | Funcionar em vários tamanhos de tela | Regras responsivas no CSS; falta evidência de smartphone | IMPLEMENTADO |
| RF26 | Não exigir CRUD | Somente consultas GET; não há administração/cadastro | PASSA |
| RF27 | Uso de IA documentado | `USO_DE_IA.md` cobre ferramenta, finalidade, prompts, validações, erro e correção | PASSA |
| RF28 | README técnico | Objetivo, tecnologias, arquitetura, pastas, execução, banco, endpoints, modelo, responsividade, acessibilidade, publicação, limitações | PASSA |
| RF29 | Migrations/scripts | `InitialCreate` versionada e aplicada ao Render no startup | PASSA |
| RF30 | ZIP de arquivos de publicação | Job `entrega-zip` aprovado; artefato `sesi-519-publicacao` (23,5 MB, criado em 22/10/2026) disponível no GitHub Actions; baixar e conferir antes do envio | GERADO / CONFERÊNCIA FINAL PENDENTE |
| RF31 | Limitações/diferenças ao Figma identificadas e justificadas | Limitações técnicas documentadas; diferenças visuais específicas ainda não verificáveis | PENDENTE |
| RF32 | Link do GitHub disponível aos avaliadores | Repositório privado na última verificação; necessidade de acesso de avaliadores | PENDENTE |
| RF33 | Integrar Moodle | Edital diz que é diferencial opcional, não obrigatório | OPCIONAL |

## 3. Achados por prioridade

### CRÍTICO — Bloqueios antes de entregar

**A-01 — Falta de aferição de fidelidade ao Figma.** O edital define o Figma como referência visual e de conteúdo. Não é suficiente a interface “estar bonita”. Precisamos comparar telas (desktop e mobile quando houver), tipografia, identidade, cores, componentes, informações fictícias, espaçamento, e registrar decisões e diferenças. Acesso à API do arquivo foi negado, e não foi fornecida uma captura comparável nesta auditoria.

**A-02 — Repositório privado.** Na última consulta, o repo continua `private`. A entrega por URL só funciona se os avaliadores tiverem acesso. Confirmar instruções para habilitar acesso a avaliadores ou tornar público **somente se isso for permitido pelo processo seletivo e pela confidencialidade dos anexos**. Não alterar visibilidade sem decisão explícita.

**A-03 — ZIP exigido pelo edital.** O edital pede uma relação de arquivos de publicação em formato ZIP. O job `entrega-zip` gerou com sucesso `sesi-519-publicacao.zip`, disponível no artefato `sesi-519-publicacao` na execução [39774064375](https://github.com/juceliocoelho2022/sesi-catalogo-cursos/actions/runs/39774064375). Antes do envio, baixar e conferir se o pacote contém aplicação publicada (`app/`), arquivo `codigo-fonte.zip`, documentos e configuração Docker.

### IMPORTANTE — Qualidade e robustez

**A-04 — Responsividade e acessibilidade sem evidência.** O CSS contém grades para 3/2/1 colunas, labels e foco visível. Ainda falta teste em 375 px e 768 px, teclado (Tab, Enter, Escape quando aplicável), mensagens em leitor de tela, contraste e overflow horizontal.

**A-05 — Dados pouco diversos em módulos.** No `DbSeeder`, todos os seis cursos usam os títulos `Fundamentos` e `Desenvolvimento` e estrutura fixa de 3 aulas. As descrições variam principalmente pelo nome do curso. O texto oficial pede cursos com diferentes módulos e descrições de aulas; recomenda-se diversificar módulos/ementas coerentes com cada categoria, priorizando os conteúdos realmente mostrados no Figma, e propagar mudanças ao banco remoto por migration/seed controlado.

**A-06 — Fonte distribuída, mas não usada.** O ZIP oficial contém fontes Open Sans e licença OFL. O CSS usa `Arial`. Isso **não comprova** divergência do Figma, mas deve ser verificado e, caso o protótipo use Open Sans, ajustado com inclusão correta de licenciamento.

**A-07 — Recursos visuais repetidos.** As seis ofertas reutilizam três imagens do ZIP (`hero-image.png`, `banner-ava.png`, `video-thumb.png`). Verificar se esse uso é fiel à apresentação de cada curso no Figma.

**A-08 — Backup temporário versionado.** `App.tsx.bak` e `style.css.bak` foram encontrados no GitHub. Foram removidos na branch auditada.

**A-09 — API funcional, mas inicialmente apenas CI de build.** Foram acrescentados 12 cenários de contrato HTTP via `tests/smoke-api.sh` e um job de integração no GitHub Actions com PostgreSQL 17 real em container. **A execução 39774064375 foi aprovada**: os cinco jobs (backend, frontend, Docker, integração e ZIP) terminaram com sucesso.

**A-10 — Escopo operacional da hospedagem gratuita.** Web Service Free pode hibernar, e o banco gratuito do Render vence em 21/11/2026. A página de validação e as instruções locais precisam permanecer acessíveis para a avaliação.

### MELHORIAS — Não bloquear requisitos essenciais

- O endpoint `/health` verifica apenas o processo, não a conectividade do PostgreSQL; a consulta `/api/cursos` é o teste de ponta a ponta.
- A pesquisa SQL usa `ILIKE` com padrões `%...%`; caracteres especiais de curingas (`%` e `_`) ainda não são tratados como texto literal; melhorar para busca robusta.
- Não há evidência de testes automatizados de teclado/mobile (necessário ao menos testes manuais documentados).
- Evitar prometer conformidade WCAG ou paridade pixel a pixel sem evidência.
- Evitar atualizações constantes após aprovar e publicar a versão final; cada push para a branch do Render pode gerar novo deploy.

## 4. Documentos e entregáveis

| Entrega do PDF oficial | Onde está / próximo passo |
| --- | --- |
| Link GitHub | <https://github.com/juceliocoelho2022/sesi-catalogo-cursos> — checar acesso dos avaliadores |
| Link aplicação | <https://sesi-catalogo-519-web.onrender.com/> — Live e validada |
| Frontend | `frontend/` no GitHub |
| Backend | `backend/CatalogoCursos.Api/` no GitHub |
| Migrations | `backend/CatalogoCursos.Api/Migrations/` |
| README | `README.md` |
| Registro de IA | `USO_DE_IA.md` |
| ZIP de publicação | Artefato `sesi-519-publicacao` disponível em [GitHub Actions 39774064375](https://github.com/juceliocoelho2022/sesi-catalogo-cursos/actions/runs/39774064375); baixar e conferir |
| Relação de limitações | Seção README e este documento; falta comparação definitiva com Figma |

## 5. Ações corretivas já executadas nesta auditoria

- Removidos os arquivos `frontend/src/App.tsx.bak` e `frontend/src/style.css.bak`, que eram resíduos de edição.
- Criado `tests/smoke-api.sh` com 12 cenários verificáveis.
- Implementado job `integration` usando PostgreSQL real no GitHub Actions; **aprovado**.
- Implementado job `entrega-zip`, produzindo artefatos de publicação compilados e arquivo de código-fonte; **aprovado**.
- Alinhado `render.yaml` à região e ao nome do banco realmente criados na conta (`virginia`, `sesi_catalogo_519_db`).

## 6. Plano de aceite objetivo

**Primeiro:** CI GREEN confirmado para backend, frontend, container, integração e ZIP na execução 39774064375; falta baixar e inspecionar o ZIP.  
**Segundo:** conferir manualmente o Figma oficial lado a lado com a URL Render e registrar eventuais diferenças justificadas.  
**Terceiro:** testar mobile, teclado, loading, erro e imagens, com captura de evidência.  
**Quarto:** conceder acesso apropriado dos recrutadores ao GitHub, sem divulgar secrets ou material restrito indevidamente.  
**Quinto:** baixar e verificar o ZIP, integrar a branch aprovada na `main`, conferir novo deploy e enviar links antes de 26/10/2026 às 08h.

**Veredicto:** **apto tecnicamente à demonstração, ainda não homologado integralmente contra o edital.**
