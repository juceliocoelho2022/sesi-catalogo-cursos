# Correspondência visual com o Figma — [TEST DEV] SESI Indústria

**Revisão:** 22/10/2026  
**Referência visual:** [protótipo oficial Figma](https://www.figma.com/proto/YB0o91nnOEB5bXXCtmy5s6/-TEST-DEV--SESI-Ind%C3%BAstria?node-id=6017-342&p=f&viewport=-1570%2C411%2C0.09&t=tvoKbn8KIvr7vLak-1&scaling=contain&content-scaling=fixed&starting-point-node-id=6017%3A342&show-proto-sidebar=1&page-id=3%3A2).
**Implementação:** <https://sesi-catalogo-519-web.onrender.com/>.

## 1. Como a referência foi inspecionada

O protótipo foi aberto publicamente no modo de apresentação, por navegação automatizada, sem modificar seus arquivos. Foram identificados os fluxos **01. Landing Page**, **02. Detalhes Curso** e **03. AVA**. A API de design do Figma não permitiu ler propriedades detalhadas do arquivo; portanto, esta auditoria compara **elementos visuais e conteúdo observáveis no protótipo**, e não medidas exatas de coordenadas, tamanhos, cores ou estilos internos.

## 2. Comparação por área

| Elemento do Figma | Implementação do catálogo | Resultado / justificativa |
| --- | --- | --- |
| Logo SESI + INDÚSTRIA | Logotipo oficial disponibilizado nos assets | Implementado |
| Header com navegação Início, Cursos, Sobre | Header com âncoras funcionais para início, catálogo e seção Sobre | Implementado na revisão visual |
| Link "Área do aluno", login e Google | Não implementado | O edital não exige autenticação, cadastro de estudantes ou Moodle; não criamos links que não funcionam |
| Landing hero vermelho e imagem industrial | Hero com gradiente vermelho, fotografia `hero-image.png` e texto inspirado no Figma | Implementado na revisão; fidelidade pixel a pixel não validada |
| Título "Impulsione sua carreira na indústria" | Usado como destaque da landing | Implementado |
| CTA "Conhecer cursos" | Âncora real para seção do catálogo | Implementado |
| Seção "Principais cursos" | Identifica grade de seis cursos, mantendo título "Explore nossos cursos" | Implementado e ampliado para requisito do edital |
| Um curso de exemplo "Matemática para a Precisão: Medidas e Escalas na Metalurgia" | Seis cursos fictícios com nomes e áreas variadas | Diferença assumida: o desafio exige no mínimo seis cursos; a página demonstra coleção fictícia, não um clone de um único curso |
| Detalhes do curso em tela própria com imagem e certificação | Detalhes expandem dentro do próprio card com público-alvo, carga horária, data, módulos e aulas | Diferença deliberada de UX: aprovado pelo candidato; cumpre "abrir detalhes" mas não reproduz todo o layout da tela do Figma |
| Botão "Matricule-se" | Não incluído | Matrícula não está listada entre funcionalidades obrigatórias; evitar simular processo real inexistente |
| Quatro benefícios ("Certificação SESI", "Professores especialistas", "Suporte contínuo", "Flexibilidade") | Não estão reproduzidos em um bloco de marketing | Diferença visual conhecida, não implementada; assets dos ícones estão disponíveis |
| AVA com progresso, boas práticas e recomendações | Fora do escopo do catálogo demonstrativo | Moodle/AVA é diferencial opcional, não requisito mínimo; não simular progresso do aluno |
| Cores de destaque vermelho, fundo branco, preto/cinza | Usadas na landing e no catálogo | Implementação inspirada nas cores observadas |
| Tipografia e espaçamento | Tipografia sans-serif e grade responsiva | Sem aferição objetiva de fontes e medidas; auditoria manual pendente |

## 3. O que foi efetivamente alterado

- Incluído cabeçalho com navegação acessível e links funcionais (`#inicio`, `#cursos`, `#sobre`).
- Incluída landing page vermelha e fotografia industrial, reaproveitando o asset disponibilizado.
- Incluídos título e texto principais inspirados no Figma e CTA que leva ao catálogo.
- Preservados os seis cursos, pesquisa, filtros combinados, contagem, estado vazio e detalhe dentro do card.
- Melhorado o formato de exibição da data no detalhe (`dd/mm/aaaa`), preservando o formato original da API.
- Adicionado link de salto ao conteúdo e regra para usuários que preferem reduzir animação.

**Nenhum recurso fictício de matrícula, login, pagamento ou progresso foi inventado.**

## 4. Testes que ainda precisam ser executados manualmente

1. Comparar a página pública com a **Landing Page** do Figma lado a lado, em desktop.
2. Validar espaçamentos, tipografia, posicionamento dos elementos e tratamento da imagem.
3. Testar 375 px, 768 px e 1366 px, verificando rolagem horizontal, legibilidade e navegação.
4. Testar Tab/Shift+Tab, CTA, filtros, chips de remoção e detalhes dos cards; verificar foco visível.
5. Checar o contraste real de textos/botões e leitores de tela, sem presumir conformidade WCAG.
6. Confirmar que ao clicar em "Ver detalhes" o painel segue dentro do card, sem cobrir outros elementos.
7. Rever se algum item visual do protótipo precisa ser explicitamente documentado como diferença adicional.

## 5. Posição de conformidade

O desafio pede **frontend baseado no Figma**; a reprodução completa da AVA ou funcionalidades de matrícula não são exigidas. A opção adotada foi preservar a identidade, o fluxo de apresentação e o objetivo de consulta de cursos, enquanto amplia o catálogo para os mínimos obrigatórios.

A aderência está **parcialmente validada por inspeção visual do protótipo e do código**. Não alegar equivalência pixel a pixel nem conclusão da auditoria de acessibilidade sem execução manual e evidências.
