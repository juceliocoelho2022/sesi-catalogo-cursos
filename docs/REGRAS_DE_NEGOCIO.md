# Regras de negócio e requisitos funcionais
**Catálogo de Cursos | Case 519/2026 | SESI/SENAI-SP**

> Documento elaborado para explicar a proposta aos avaliadores. Separa **exigências do desafio**, **decisões de implementação** e **pontos ainda não implementados**. Não representa regras de um Moodle real.

## 1. Objetivo e atores

**Objetivo:** permitir que o visitante consulte ofertas fictícias de cursos de modo simples, acessível e responsivo, sem autenticação.

**Ator principal:** visitante do catálogo.  
**Atores técnicos:** desenvolvedor/avaliador, que inicia a API, configura o banco e verifica o funcionamento.  
**Fora do escopo:** administrador de cursos, matrícula, aluno autenticado, pagamento, integração real com Moodle e gerenciamento de cursos.

## 2. Glossário de domínio

| Termo | Definição |
| --- | --- |
| Categoria | Área de conhecimento que classifica um curso; na base fictícia: Matemática, Linguagens e Ciências da Natureza |
| Curso | Oferta educativa com nome completo, nome breve, descrição, público-alvo e características de disponibilização |
| Modalidade | Forma de oferta; no seed inicial: Online ou Presencial |
| Status | Estado de disponibilidade; no seed: Disponível, Em breve, Encerrado |
| Módulo | Agrupador de aulas de um curso, com ordem de exibição |
| Aula | Conteúdo vinculado a um módulo, com título, descrição e ordem |
| Pesquisa | Busca textual pelos nomes completo e breve |
| Filtros | Restrição adicional por categoria, modalidade ou status |

## 3. Requisitos fornecidos no desafio

**Obrigatórios:** backend C#/.NET, API de cursos, banco aberto, integração com frontend, responsividade, acessibilidade, listagem, pesquisa, filtros cumulativos, contagem, limpar filtros, detalhes, estados de carregamento/erro/vazio, dados fictícios e documentação. Base mínima: seis cursos, três categorias, duas modalidades e três status distintos, com módulos, aulas, públicos e cargas horárias variados.

**Diferencial opcional:** integração real com Moodle.  
**Dispensado:** cadastro, edição e exclusão de cursos.  
**Obrigação de entrega:** GitHub, aplicação publicada gratuitamente, scripts/migrations, README, USO_DE_IA.md, ZIP de publicação e registro das limitações.

## 4. Regras de negócio (RN)

### RN-01 — Catálogo com dados de demonstração
- **Origem:** exigência do desafio.
- **Regra:** disponibilizar pelo menos 6 cursos fictícios, 3 categorias, 2 modalidades e 3 status.
- **Implementação atual:** `DbSeeder` inicializa 6 cursos, 3 categorias, 2 modalidades e 3 status.
- **Aceitação:** `GET /api/cursos` sem parâmetros retorna `total=6` na base inicial.
- **Situação:** consulta básica validada pelo candidato.

### RN-02 — Atributos obrigatórios de apresentação
- **Origem:** exigência do desafio.
- **Regra:** cada curso apresenta nome completo, nome breve, descrição, categoria, modalidade, carga horária, público-alvo, status e imagem; data de disponibilização quando aplicável.
- **Implementação atual:** essas propriedades aparecem em `Curso` e DTOs; data aceita valor nulo.
- **Aceitação:** conferir valores coerentes nos 6 objetos do endpoint de listagem.
- **Observação:** a versão atual não possui validação formal de campos obrigatórios via endpoints de escrita, pois não há cadastro.

### RN-03 — Categoria por relacionamento
- **Origem:** decisão de modelagem.
- **Regra:** cada curso pertence a uma categoria; uma categoria agrupa vários cursos.
- **Implementação atual:** relação `Categoria 1:N Curso`, chave estrangeira `CategoriaId`, índice único em `Categoria.Nome`.
- **Aceitação:** listar cursos por categoria e constatar vínculo válido.

### RN-04 — Identificação do curso
- **Origem:** decisão técnica.
- **Regra:** curso identificado por `Id` numérico; nome breve único na base.
- **Implementação atual:** PK `Id` e índice único em `NomeBreve`.
- **Atenção:** **não** há índice de unicidade em `NomeCompleto`; não alegar esse requisito.

### RN-05 — Pesquisa textual por nome
- **Origem:** exigência do desafio + decisão de implementação.
- **Regra:** pesquisar parte do **nome completo OU do nome breve**, sem diferenciar letras maiúsculas/minúsculas.
- **Implementação atual:** `EF.Functions.ILike` no PostgreSQL com padrão `%termo%`; `Trim()` no termo de entrada.
- **Aceitação:** `?pesquisa=Matem%C3%A1tica` encontra *Matemática Aplicada ao Cotidiano* (1 na base inicial).
- **Caso importante:** *Geometria e Visualização Espacial* pertence à categoria Matemática, mas não contém essa palavra no nome e **não** deve aparecer nessa pesquisa. Isso diferencia pesquisa de filtro por categoria.

### RN-06 — Filtragem por categoria
- **Origem:** exigência do desafio.
- **Regra:** ao selecionar uma categoria, retornar apenas cursos vinculados a ela.
- **Implementação atual:** igualdade com `Categoria.Nome` após `Trim()`.
- **Aceitação:** `?categoria=Matem%C3%A1tica` retorna 2; `?categoria=Linguagens` retorna 2.
- **Nota:** comparação exata conforme implementação; não prometemos normalização de acentos ou grafias semelhantes para os filtros.

### RN-07 — Filtragem por modalidade
- **Origem:** exigência do desafio.
- **Regra:** selecionar modalidade restringe cursos àquela modalidade.
- **Implementação atual:** igualdade textual com `Curso.Modalidade`.
- **Aceitação:** `?modalidade=Online` retorna 3 na base de demonstração.

### RN-08 — Filtragem por status
- **Origem:** exigência do desafio.
- **Regra:** selecionar status restringe o resultado ao valor correspondente.
- **Implementação atual:** igualdade textual com `Curso.Status`.
- **Aceitação:** `?status=Dispon%C3%ADvel` retorna 3 na base inicial.
- **Limitação:** valores não possuem enum/constraint formal; a lista de opções é derivada dos dados existentes.

### RN-09 — Combinação de critérios (AND)
- **Origem:** exigência do desafio.
- **Regra:** os campos preenchidos são cumulativos; um curso só aparece se satisfizer **todos** os critérios informados.
- **Implementação atual:** condições `Where` aplicadas sequencialmente no IQueryable.
- **Aceitação:** pesquisa `Matemática` + categoria `Matemática` + modalidade `Online` + status `Disponível` → 1 resultado.
- **Contraexemplo:** pesquisa `Matemática` + categoria `Linguagens` → 0, **comportamento correto**.

### RN-10 — Contagem coerente dos resultados
- **Origem:** exigência do desafio.
- **Regra:** mostrar número de cursos retornados pelos critérios atuais.
- **Implementação atual:** `{total, itens}` na API; a interface apresenta a quantidade de cards.
- **Aceitação:** nenhum filtro = 6; Linguagens isolado = 2; combinação sem correspondência = 0.

### RN-11 — Limpar ou remover critérios
- **Origem:** exigência do desafio + melhoria de UX.
- **Regra:** botão **Limpar filtros** remove pesquisa, categoria, modalidade e status; a interface também mostra chips removíveis de filtros ativos.
- **Aceitação:** após limpar, `total=6` na base inicial; retirar chip de pesquisa mantém categoria e demais critérios.
- **Situação:** chips implementados na branch de desenvolvimento; validação no navegador pendente.

### RN-12 — Detalhes de curso
- **Origem:** exigência do desafio.
- **Regra:** visitante consulta mais informações de um curso pelo `Id`, incluindo público-alvo, módulos e aulas.
- **Implementação atual:** `GET /api/cursos/{id}` com projeção em `CursoDetalheDto`; módulos e aulas ordenados pelo campo `Ordem`.
- **Aceitação:** `/api/cursos/1` retorna curso, 2 módulos e 3 aulas na base inicial; `/api/cursos/99999` retorna HTTP 404 com `{"mensagem":"Curso não encontrado."}`.

### RN-13 — Expansão contextual do card
- **Origem:** melhoria solicitada pelo candidato durante teste da interface.
- **Regra:** clique em **Ver detalhes** expande o painel **dentro do card correspondente**, e não abaixo de toda a lista de cursos.
- **Implementação atual:** estado `cursoAbertoId` identifica o card expandido; o painel é renderizado dentro do `article` escolhido.
- **Aceitação:** abrir curso A → painel aparece imediatamente abaixo das informações de A; abrir curso B → A fecha e B abre; clicar novamente em B → B fecha.
- **Acessibilidade prevista:** `aria-expanded`, `aria-controls` e estado de carregamento.
- **Situação:** alteração em branch de desenvolvimento; teste visual e responsivo pendente.

### RN-14 — Resultado vazio
- **Origem:** exigência do desafio.
- **Regra:** a interface não pode apresentar uma tela silenciosamente em branco quando a consulta não retorna cursos.
- **Implementação atual:** texto “Nenhum curso encontrado” e orientação para revisar/remover critérios.
- **Aceitação:** consulta com combinação impossível mostra `0 curso(s)` e mensagem adequada.

### RN-15 — Carregamento e erro
- **Origem:** exigência do desafio.
- **Regra:** indicar consultas em andamento e mostrar falha amigável para a pessoa usuária quando a API estiver indisponível.
- **Implementação atual:** estado `carregando`, avisos com `role=alert`, carregamento de detalhes separado.
- **Aceitação:** abrir aplicativo com API desligada → mensagem de falha; com API disponível → resultados. Trocar filtros rapidamente não deve exibir dados de uma chamada cancelada.
- **Limitação:** não existe observabilidade/monitoramento de produção.

### RN-16 — Responsividade e acessibilidade
- **Origem:** exigência do desafio.
- **Regra:** interface utilizável em desktop, tablet e smartphone; campos rotulados, controles operáveis por teclado e estados comunicados.
- **Implementação atual:** grade CSS responsiva em 900 px/600 px, labels, foco visível, `aria-live` e alertas.
- **Aceitação:** inspecionar em larguras 375 px, 768 px e desktop; tabular pelos controles; verificar contraste e leitores de tela.
- **Situação:** medidas implementadas; auditoria WCAG e comparação exata ao Figma pendentes.

### RN-17 — Catálogo somente leitura
- **Origem:** delimitação expressa no edital.
- **Regra:** visitante pode consultar, pesquisar e visualizar detalhes; não há CRUD exposto.
- **Implementação atual:** somente rotas GET de dados; sem autenticação ou formulário de matrícula.
- **Aceitação:** Swagger não deve listar POST, PUT ou DELETE para cursos.

### RN-18 — Inicialização de dados
- **Origem:** decisão técnica.
- **Regra:** carregar a amostra fictícia apenas quando ainda não houver cursos.
- **Implementação atual:** `DbSeeder.SeedAsync` é chamado somente em `Development`, quando `Cursos.AnyAsync()` é falso.
- **Aceitação:** reiniciar a API com banco já preenchido não deve duplicar os seis cursos.
- **Limitação crítica para publicação:** ambiente `Production` não executa esse seed; é preciso prover dados por mecanismo de implantação.

### RN-19 — Integridade referencial e ordenação
- **Origem:** decisão técnica.
- **Regra:** módulo pertence a um curso; aula pertence a um módulo; exibir na ordem definida.
- **Implementação atual:** FKs `Modulo.CursoId`, `Aula.ModuloId`; arrays são projetados ordenados por `Ordem`.
- **Aceitação:** detalhamento exibe módulos 1 e 2 e as aulas em sequência, dentro de cada módulo.

### RN-20 — Independência do Moodle
- **Origem:** delimitação do desafio.
- **Regra:** aplicação deve funcionar sem instalar ou acessar Moodle.
- **Implementação atual:** dados locais no PostgreSQL; nenhuma chamada a serviço Moodle.
- **Aceitação:** instalação e execução sem credenciais/instância Moodle.

## 5. Matriz de testes funcionais

| ID | Pré-condição / ação | Resultado esperado |
| --- | --- | --- |
| CT-01 | Base inicial; GET /api/cursos | 6 cursos |
| CT-02 | Pesquisa por Matemática, sem filtros | 1 curso |
| CT-03 | Categoria Matemática, pesquisa vazia | 2 cursos |
| CT-04 | Categoria Linguagens, pesquisa vazia | 2 cursos |
| CT-05 | Modalidade Online, outros campos vazios | 3 cursos |
| CT-06 | Status Disponível, outros campos vazios | 3 cursos |
| CT-07 | Matemática + Linguagens | 0 cursos, mensagem de vazio |
| CT-08 | Limpar todos os filtros | Retorna 6 cursos |
| CT-09 | Abrir detalhes do curso 1 | Painel no card 1, módulos e aulas |
| CT-10 | Com card 1 aberto, clicar em curso 2 | Card 1 fecha; card 2 expande |
| CT-11 | Clique em Fechar detalhes | Painel desaparece |
| CT-12 | GET /api/cursos/99999 | HTTP 404 |
| CT-13 | Desligar API e tentar consultar | Mensagem de erro |
| CT-14 | Navegar por teclado e testar 375 px | Campos usáveis e painel legível |

**Estado:** CT-01, CT-03 e CT-04 possuem evidência anterior de execução via PowerShell. Os demais cenários são **casos planejados**, ainda não todos executados.

## 6. Riscos, exclusões e melhorias futuras

- Não confundir "pesquisa por nome" com "filtro de categoria".
- Não tratar opções de status e modalidade como enum validado enquanto forem apenas strings.
- Não presumir que `/health` monitora o banco.
- Completar comparação com Figma, testes automatizados e publicação.
- Refinar tratamento de falha dos metadados de filtros e indicadores de carregamento.
- Melhorias opcionais, somente após conclusão do escopo obrigatório: paginação, testes de contrato, observabilidade, autenticação/administração e integração Moodle.

## 7. Rastreabilidade com o edital

As regras RN-01, RN-02, RN-05 a RN-12 e RN-14 a RN-17 decorrem do escopo e das funcionalidades solicitadas. RN-03, RN-04, RN-13, RN-18 e RN-19 explicitam decisões adotadas para implementar esse escopo. RN-20 formaliza a opção de **não** realizar a integração Moodle, pois foi classificada como diferencial.

O documento deve acompanhar o código: alterações de comportamento devem atualizar as regras e os testes correspondentes.
