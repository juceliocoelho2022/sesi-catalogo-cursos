# Publicação gratuita — Render (React + .NET + PostgreSQL)

> **Situação verificada em 22/10/2026:** aplicação **Live** no Render em <https://sesi-catalogo-519-web.onrender.com/>. Serviços Web Free e PostgreSQL 17 Free foram criados na conta Render conectada ao repositório, e a API pública retornou seis cursos. A criação inicial foi feita diretamente pelos serviços do Render; o Blueprint `render.yaml` permanece como configuração reproduzível. A implementação da interface e os endpoints básicos foram confirmados, mas testes mobile e auditoria de fidelidade ao Figma ainda estão pendentes.

## Estratégia de implantação

Este repositório contém o arquivo `render.yaml` e um `Dockerfile` para hospedar **o catálogo completo em um único domínio**:

```text
Navegador
  https://<dominio-alocado-pelo-render>/
       |
       +-- /              React compilado (wwwroot)
       +-- /api/cursos    API ASP.NET Core
       +-- /api/filtros   API ASP.NET Core
       +-- /health        Health check do processo
                         |
                         +-- EF Core + Npgsql
                                 |
                                 +-- PostgreSQL Render Free
```

A URL real já está disponível: <https://sesi-catalogo-519-web.onrender.com/>. A URL genérica no diagrama acima é apenas um modelo.

### Por que esse formato?

A API e o React ficam na mesma origem. Isso evita CORS e URLs diferentes no frontend publicado. O `Dockerfile` usa estágios separados de build Node e .NET, executando somente o ASP.NET Core no container final. O PostgreSQL é provisionado separadamente pelo Blueprint.

## Pré-requisitos

- GitHub com o repositório `juceliocoelho2022/sesi-catalogo-cursos` atualizado;
- conta gratuita no Render (<https://dashboard.render.com/>);
- Render conectado à sua conta GitHub, com permissão de leitura do **repositório privado** do case;
- a branch que contém `render.yaml`, `Dockerfile` e os ajustes do `Program.cs`. Atualmente: `feature/filtros-ux`.

**Não informe senhas do GitHub ou do banco em conversas ou documentos.** Autorize a conexão na interface oficial do Render.

## Implantar com o Blueprint

1. No GitHub, abra o Pull Request #1 e confirme a verificação **Container - React + ASP.NET Core** aprovada.
2. Abra <https://dashboard.render.com/> e entre com sua conta.
3. No Render, vá até **New → Blueprint**.
4. Conecte o GitHub (se solicitado). Permita acesso ao repositório `sesi-catalogo-cursos`.
5. Selecione o repositório e a branch com a configuração: `feature/filtros-ux` (ou `main` **somente depois do merge**).
6. Use o arquivo `render.yaml` na raiz.
7. Revise os recursos antes de confirmar: **Web Service — Free** e **Postgres — Free**.
8. Confirme que nenhuma instância paga está selecionada. O banco de dados da oferta Free expira em 30 dias.
9. Aprove a criação do Blueprint e acompanhe os logs do build e da inicialização.
10. Copie a URL **real** `https://...onrender.com` exibida na página do serviço web; não tente adivinhar a URL com base no nome.

### Configuração automatizada pelo render.yaml

- Backend/container: `runtime: docker`, `plan: free`.
- Banco PostgreSQL 17 gratuito, criado na mesma região do serviço.
- `DATABASE_URL` é injetada como segredo pelo Render, sem armazenar senha no GitHub.
- `ASPNETCORE_ENVIRONMENT=Production`.
- `SEED_DEMO_DATA=true`: no início do processo, aplica a migration versionada e cadastra os seis cursos fictícios se ainda não existirem.
- `/health` utilizado para verificação do processo web.
- Porta de aplicação definida como **10000** no container.

A conversão da URL `postgresql://...` para a string de conexão do Npgsql é feita no `Program.cs`.

**Importante:** os serviços gratuitos podem entrar em repouso após 15 minutos sem tráfego. A primeira visita pode levar cerca de um minuto para ativar novamente. O Render Postgres gratuito expira em 30 dias; para um projeto que precisa durar mais, seria necessário migrar para outra solução. Consulte [limitações oficiais](https://render.com/docs/free).

## Verificação depois que o Render mostrar "Live"

Defina no PowerShell o **endereço real** do serviço, sem barra final:

```powershell
$site = "https://COLE-AQUI-O-ENDERECO-REAL.onrender.com"

Invoke-RestMethod "$site/health"
Invoke-RestMethod "$site/api/cursos"
Invoke-RestMethod "$site/api/cursos?categoria=Linguagens"
Invoke-RestMethod "$site/api/cursos/1" | ConvertTo-Json -Depth 10
```

Em seguida abra `$site` no navegador e verifique:
1. Página React visível em HTTPS e imagens dos cards carregadas.
2. Seis cursos sem filtro.
3. Categoria Matemática retorna 2; categoria Linguagens retorna 2.
4. Botão **Ver detalhes** expande dentro do próprio card.
5. Pesquisa por nome e filtros combinados.
6. Teste em celular e desktop.
7. Refresh da URL principal e dos caminhos da SPA.

**Não marque a entrega como concluída apenas porque o painel do Render mostrou "Live"**: é obrigatório testar os endpoints e o frontend público.

## Como atualizar após o merge na main

O Blueprint inicialmente pode apontar para `feature/filtros-ux`. Depois que os testes forem aprovados e o Pull Request for integrado, atualize a branch do serviço no Render para `main` (ou ajuste a seleção da instância do Blueprint) e verifique a nova publicação. Não crie uma segunda instância paga ou duplicada desnecessariamente.

## Solução de problemas

| Sintoma | Verificação |
| --- | --- |
| Docker build falha | Conferir logs dos estágios `npm ci`, `npm run build`, `dotnet restore` e `dotnet publish`; comparar com GitHub Actions |
| Serviço não fica Live | Conferir `DATABASE_URL`, criação/conexão do PostgreSQL, migrations e porta de escuta |
| `/health` responde mas `/api/cursos` não | Ver logs da API e do PostgreSQL; health check atual não consulta o banco |
| Página abre mas sem cursos | Verificar seed `SEED_DEMO_DATA=true` e se migration `InitialCreate` foi aplicada |
| Cards sem imagem | Confirmar que `frontend/public/assets` foi copiado para `wwwroot/assets` |
| 404 na raiz | Verificar etapa de build do React e `app.UseDefaultFiles()`, `app.UseStaticFiles()` |
| CORS | Frontend e API publicados devem usar **o mesmo domínio**; não configure `VITE_API_URL` externo no build |
| Primeiro acesso lento | Render Free hiberna e pode demorar a inicializar |

## Checklist de entrega oficial

- [ ] Serviço web e banco criados **sem custos** no plano Free
- [ ] URL pública testada (site, API, imagens e filtros)
- [ ] Repositório GitHub acessível aos avaliadores segundo as regras do processo
- [ ] Código do backend e frontend versionados
- [ ] Migrations versionadas
- [ ] README com URL publicada efetiva
- [ ] USO_DE_IA.md atualizado
- [ ] Diferenças com o Figma e limitações documentadas
- [ ] Artefatos de publicação em ZIP
- [ ] Envio antes das 08h de 26/10/2026 (horário de Brasília)

**Publicação confirmada em 22/10/2026.** A entrega final ainda depende da documentação do Figma, da disponibilização do repositório aos avaliadores e do ZIP solicitado no case.
