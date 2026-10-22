# Homologação responsiva — SESI/SENAI 519/2026

**Data:** 22/10/2026  
**Ambiente:** aplicativo publicado no Render, aberto no Chrome DevTools em emulação móvel de 390 px.  
**Origem das evidências:** capturas de tela realizadas pelo candidato durante a homologação manual.  
**URL validada:** https://sesi-catalogo-519-web.onrender.com/

## Matriz de aceite mobile

| Cenário | Evidência observada | Resultado |
| --- | --- | --- |
| Hero e navegação | Captura em viewport móvel mostrando marca, links, banner, título e CTA | Aprovado visualmente |
| Formulário de filtros | Campos de pesquisa, categoria, modalidade e status empilhados; limpar filtros acessível | Aprovado visualmente |
| Scroll horizontal (antes de expandir) | Console do DevTools exibiu `window.innerWidth=390`, `document.documentElement.scrollWidth=390`, `overflowHorizontal=false` | Aprovado |
| Grade de cursos | Captura mostra cards um abaixo do outro, com imagens e botões dentro dos limites visuais | Aprovado visualmente |
| Detalhe dentro do card | Captura mostra curso Geometria e Visualização Espacial com botão `Fechar detalhes`, dados de oferta, módulos e aulas logo abaixo do botão no mesmo card | Aprovado visualmente |
| Scroll horizontal após expandir | A última captura não contém nova avaliação do Console para `scrollWidth` no estado expandido; não há corte evidente na área capturada | Sem medição conclusiva |
| Teclado/leitor de tela | Não foi apresentada execução manual acessível | Pendente |
| Tablet 768 px | Não foi apresentada captura nem auditoria | Pendente |

## Decisões

- A expansão dos detalhes **dentro do card selecionado** foi mantida, conforme aceite do candidato.
- Nenhuma mudança de layout adicional será feita sem evidência de defeito.
- A interface deve continuar a ser descrita como **responsiva em testes manuais desktop/390 px**, sem alegar homologação WCAG, tablet ou equivalência pixel a pixel com o Figma.
- Para demonstração de entrevista, mostrar filtros combinados, mensagem de resultados vazios, detalhamento inline e API PostgreSQL.

## Relatórios complementares

- [Matriz do edital e pendências](AUDITORIA_CONFORMIDADE_519_2026.md)
- [Diferenças justificadas com o Figma](DIFERENCAS_FIGMA.md)
- [Roteiro de demonstração](ROTEIRO_DEMONSTRACAO.md)

## Riscos e entrega

O repositório está **privado** na última consulta; garantir acesso ao avaliador antes do envio. O serviço Render Free pode hibernar, razão pela qual a execução local é documentada no README. O ZIP de publicação é gerado pelo workflow de CI e precisa ser conferido pelo candidato antes de ser anexado ou compartilhado.
