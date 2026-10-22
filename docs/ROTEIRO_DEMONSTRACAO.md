# Roteiro de demonstração técnica — SESI/SENAI-SP 519/2026

**Objetivo:** apresentar, em aproximadamente 7–10 minutos, uma aplicação web de catálogo de cursos, com requisitos rastreáveis e decisões técnicas justificadas. Demonstre **somente o que estiver funcionando no ambiente**.

## 1. Preparação antes da entrevista

Abra três terminais ou três abas no VS Code.

**Terminal A — PostgreSQL**
```powershell
cd E:\sesi-catalogo-cursos-etapa1\sesi-catalogo-cursos
docker compose up -d postgres
docker compose ps
```

**Terminal B — API**
```powershell
cd E:\sesi-catalogo-cursos-etapa1\sesi-catalogo-cursos\backend\CatalogoCursos.Api
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --urls http://localhost:5080
```

**Terminal C — Interface**
```powershell
cd E:\sesi-catalogo-cursos-etapa1\sesi-catalogo-cursos\frontend
npm run dev
```

Mantenha abertas as abas `http://localhost:5173` e `http://localhost:5080/swagger`. Verifique previamente acesso à aplicação publicada, quando existir, e prepare a documentação para consulta.

## 2. Abertura — 45 segundos

**Sugestão de fala:**

> Desenvolvi um catálogo de cursos para o cenário proposto pelo CTED. O foco foi atender à consulta de ofertas educacionais com busca, filtros combinados, detalhes e uma arquitetura simples de manter. A solução utiliza React e TypeScript no frontend, ASP.NET Core no backend e PostgreSQL para persistência. O catálogo foi concebido como somente leitura, porque o edital não solicitou operações de cadastro ou edição.

## 3. Demonstração da interface — 2 minutos

1. Mostre o catálogo com seis cursos.
2. Destaque o campo de pesquisa e os filtros de categoria, modalidade e status.
3. Digite `Matemática`: a pesquisa por **nome** exibe um curso.
4. Limpe a pesquisa e selecione a categoria `Matemática`: aparecem **dois** cursos.
5. Explique que *Geometria e Visualização Espacial* pertence à categoria Matemática, mas não contém essa palavra no título.
6. Selecione `Linguagens` com a pesquisa vazia: dois cursos.
7. Combine uma busca incompatível com um filtro e mostre a mensagem de vazio e os filtros ativos.
8. Limpe os filtros e retorne a seis cursos.

**Fala técnica sugerida:** “A composição das condições acontece no backend. O cliente envia os parâmetros de consulta; o banco filtra os registros e devolve apenas o resultado.”

## 4. Detalhamento do curso — 1 minuto

1. Clique em **Ver detalhes** no card *Matemática Aplicada ao Cotidiano*.
2. Mostre que o painel **se expande dentro desse card**, com nome breve, modalidade, carga horária, público-alvo, data, módulos e aulas.
3. Clique em outro curso. O primeiro painel deve fechar e o segundo abrir no novo card.
4. Clique em **Fechar detalhes** e demonstre o recolhimento.

**Fala técnica:** “Essa melhoria veio de um teste de usabilidade. O usuário não precisa percorrer a página até o fim para localizar os detalhes; as informações permanecem no contexto do curso selecionado.”

## 5. Backend, persistência e Swagger — 2 minutos

Na aba do Swagger:
- `GET /health`: verifica se o processo responde; não é health check do banco.
- `GET /api/cursos`: lista e combina parâmetros opcionais.
- `GET /api/cursos/{id}`: retorna também módulos e aulas, em ordem.
- `GET /api/filtros`: retorna opções existentes na base.

No código, mostre:
- `CursosController.cs`: consultas `AsNoTracking`, `ILike`, filtros condicionais, DTOs e `404`.
- `CatalogoDbContext.cs`: relacionamentos entre entidades e índices únicos.
- `DbSeeder.cs`: dados fictícios de apresentação.
- `Migrations/`: evolução do esquema de banco com Entity Framework Core.

**Fala técnica:** “A camada REST não devolve diretamente as entidades. Uso DTOs específicos para listagem e detalhes, reduzindo acoplamento entre persistência e apresentação.”

## 6. Arquitetura e decisões — 1 minuto

```text
React / TypeScript --> ASP.NET Core --> EF Core --> PostgreSQL
```

Justificativas:
- C# e ASP.NET Core atendem à tecnologia exigida.
- PostgreSQL é um banco relacional open source apropriado às entidades e relacionamentos do case.
- A separação frontend/API facilita evolução e testes.
- O uso de consultas sem tracking evita custo de rastreamento em um catálogo somente leitura.
- Não implementar CRUD ou Moodle no MVP protege o prazo e o escopo obrigatório.

## 7. Tratamento de erros e acessibilidade — 1 minuto

- Demonstre `Nenhum curso encontrado`.
- Se for seguro e a apresentação estiver controlada, interrompa temporariamente a API para mostrar a mensagem amigável de indisponibilidade e depois reinicie-a.
- Ajuste a largura do navegador para demonstrar a grade de 3, 2 e 1 coluna.
- Navegue pelos campos com Tab; use Enter/Space para acionar os botões.

**Não declarar conformidade WCAG plena** até concluir a auditoria correspondente.

## 8. Qualidade, IA e limitações — 1 minuto

- Mostre `README.md`, `docs/REGRAS_DE_NEGOCIO.md` e `USO_DE_IA.md`.
- Explique que a IA ajudou na estruturação e documentação, mas o código foi verificado em ambiente local.
- Cite o problema de interpretação da pesquisa por Matemática e a correção de comunicação na interface.
- Seja claro sobre itens ainda não finalizados: publicação, evidência de testes automatizados e conferência visual quadro a quadro com o Figma.

## 9. Perguntas que podem surgir

**Por que usar um banco relacional?** Porque Categoria–Curso–Módulo–Aula forma uma hierarquia com chaves estrangeiras, ordenação e integridade referencial.

**Qual a diferença entre pesquisa e filtro?** Pesquisa usa `ILIKE` nos nomes; filtro de categoria corresponde ao nome da categoria. Critérios diferentes são aplicados com AND.

**Por que não criar CRUD?** Porque o desafio explicitamente dispensa cadastro, edição e exclusão. O tempo foi direcionado ao fluxo de consulta.

**Como evitar requisições desatualizadas?** A listagem cancela consultas anteriores ao mudar os filtros, e a abertura de outro curso cancela a consulta de detalhe anterior.

**Como executar em outro computador?** Docker Compose para banco, `dotnet ef database update` para esquema, API e frontend com comandos documentados no README.

**Quais riscos existem para produção?** CORS precisa ser configurado para o domínio publicado, a string de conexão deve ser secreta, o seed só roda em Development e é necessária uma infraestrutura gratuita estável.

## 10. Checklist final de entrega

- [ ] `npm run build` e `dotnet build` aprovados após as últimas mudanças
- [ ] Testar os filtros combinados e detalhes inline
- [ ] Testar telas pequenas e navegação por teclado
- [ ] Comparar com Figma e documentar diferenças
- [ ] Criar/executar testes automatizados
- [ ] Publicar gratuitamente frontend, API e dados
- [ ] Atualizar README com URLs públicas e limitações
- [ ] Completar USO_DE_IA.md com prompts, descarte, validação e erro real
- [ ] Gerar ZIP de publicação
- [ ] Entregar até **26/10/2026 às 08h**, horário de Brasília

> **Orientação:** antes de apresentar, atualize o roteiro com evidências reais de testes e publicação. Não apresente um item “planejado” como funcionalidade já homologada.
