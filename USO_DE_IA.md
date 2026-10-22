# USO_DE_IA.md — Registro de inteligência artificial

Documento solicitado no desafio SESI/SENAI-SP 519/2026. Deve refletir o que ocorreu de fato durante o desenvolvimento.

## Ferramenta utilizada

ChatGPT (OpenAI), como apoio à análise dos requisitos, sugestões de arquitetura, criação de código, documentação e revisão da usabilidade.

## Objetivos de utilização

1. Traduzir as instruções do case em arquitetura e backlog.
2. Propor API ASP.NET Core + PostgreSQL + React/TypeScript.
3. Sugerir implementação de pesquisa e filtros combinados, tratamento de erro, carregamento e detalhes de cursos.
4. Preparar README técnico, regras de negócio e roteiro de demonstração.
5. Analisar problemas detectados no teste manual.

## Prompts relevantes e decisões

| Solicitação / prompt representativo | Aproveitado | Descartado ou alterado |
| --- | --- | --- |
| "Vamos estruturar o projeto completo com ASP.NET Core, React, TypeScript e PostgreSQL (...) começando pelo backend" | Arquitetura e modelagem das quatro entidades; divisão backend/frontend | Não incluir complexidade de microsserviços; adotar somente leitura |
| "Matemática só tem 1 curso e Linguagens não aparece nada" | Investigar diferenças entre pesquisa textual e filtro por categoria | Descartar a suposição inicial de que pesquisar "Matemática" deveria mostrar toda a categoria |
| "Para evitar erros vamos criar um repositório no GitHub" | Versionamento, branch de desenvolvimento e Pull Request | Evitar alterar diretamente a main antes de validar |
| "Quero o README explicado e detalhado e as regras de negócios" | Documentação orientada à apresentação técnica, riscos e critérios de aceitação | Não atribuir testes não executados à implementação |
| "Ao clicar em Ver detalhes quero que o detalhes abra abaixo do curso escolhido, não abaixo de todos os cursos" | Painel de detalhes diretamente dentro do card selecionado | Abandonar posicionamento de detalhes após a grade inteira |

## Como o conteúdo foi validado

**Validações efetivamente reportadas pelo candidato no Windows:**
- `dotnet build` aprovado com SDK 10.0.402 (o build inicial apresentou alertas de pacotes, tratados em etapa posterior a verificar).
- `GET /health` retornou `status: ok`.
- `GET /api/cursos` retornou `total: 6`.
- `GET /api/cursos?categoria=Matemática` retornou dois cursos.
- `GET /api/cursos?categoria=Linguagens` retornou dois cursos.
- Interface React acessível e exibindo cards, verificada por captura de tela.

**Itens ainda a validar após esta alteração de branch:**
- `npm ci` / `npm run build` na versão revisada do frontend;
- abertura/fechamento de detalhes dentro do card;
- filtros ativos e remoção individual; layout em telas móveis;
- testes automatizados, fidelidade ao Figma e publicação gratuita.

Não registrar testes como aprovados sem executar e guardar a evidência correspondente.

## Exemplo real de erro ou inadequação identificado e correção

**Inadequação identificada:** foi previsto incorretamente que a busca textual por `Matemática` retornaria dois cursos. Porém, um dos cursos da categoria tem nome `Geometria e Visualização Espacial`. Como a API pesquisa no nome completo/breve, e não na categoria, o resultado esperado da busca é **um** curso. A consulta por categoria `Matemática` retorna **dois**.

**Diagnóstico:** confirmação por consultas PowerShell à API de categoria, que retornaram dois registros tanto para Matemática quanto para Linguagens. O caso sem resultados em Linguagens pode ocorrer quando existe outro filtro incompatível ativo.

**Correção aplicada:** preservar a semântica correta de busca/filtros no backend e melhorar a comunicação no frontend, exibindo filtros ativos, mensagens de ausência de resultados e orientação explícita sobre busca textual versus categoria.

**Outra inadequação de usabilidade:** o painel de detalhes originalmente aparecia ao fim da grade. Alteração proposta/implementada na branch de desenvolvimento: renderizar o conteúdo de detalhes dentro do card clicado. A validação visual final dessa alteração está pendente no Windows do candidato.

## Responsabilidade humana

A IA fornece sugestões. A seleção das soluções, validação, configuração do ambiente, comparação com protótipo, avaliação de limitações e apresentação técnica são responsabilidade do candidato. O código, os documentos e os resultados observados precisam ser revisados antes da entrega.

## Publicação validada em 22/10/2026

O assistente auxiliou na preparação de `Dockerfile`, `render.yaml` e dos ajustes de produção. O candidato autorizou a criação dos recursos gratuitos. A implantação real foi executada na conta Render, usando Docker e PostgreSQL 17 Free. O primeiro deploy falhou por conexão com `127.0.0.1:5433`; após vincular a variável de ambiente `DATABASE_URL` ao serviço web, a nova implantação ficou **Live**.

**Validação efetiva:** página inicial React acessível por HTTPS; `GET /health` retornou `status: ok`, `GET /api/cursos` retornou seis cursos, `GET /api/filtros` retornou três categorias, duas modalidades e três status; consultas de categorias Matemática e Linguagens retornaram dois cursos cada; `GET /api/cursos/1` retornou dados e dois módulos; combinação incompatível de pesquisa e categoria retornou zero cursos.

**Endereço publicado:** <https://sesi-catalogo-519-web.onrender.com/>.

Esses testes não substituem a validação do Figma, da acessibilidade ou a entrega dos artefatos finais.
