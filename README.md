# minhas-financas-tests

Repositório dedicado exclusivamente aos testes automatizados do sistema Minhas Finanças.

## Objetivo da entrega

- Validar regras de negócio do sistema sem alterar código da aplicação alvo.
- Estruturar pirâmide de testes com backend (.NET/xUnit), frontend (Vitest) e E2E (Playwright).
- Documentar falhas encontradas com evidências reproduzíveis.

## Escopo deste repositório

- Contém apenas testes e documentação de bugs/enhancements.
- Não contém código-fonte da aplicação original.

## Pré-requisitos do ambiente

- Backend do sistema alvo rodando separadamente (repo original).
- Frontend do sistema alvo rodando separadamente (repo original) para E2E.
- .NET 9 SDK.
- Node.js 20+ e npm.

### URLs esperadas

- API: `http://localhost:5135` (ou `https://localhost:7226`)
- Web: `http://localhost:5175` (ou porta disponível equivalente)

## Estrutura inicial

- `tests/backend/unit`
- `tests/backend/integration`
- `tests/frontend/unit`
- `tests/e2e`
- `docs/bugs`
- `docs/evidence`
- `.github/workflows`

## Templates de documentação

- Bug report: `docs/bugs/BUG-template.md`
- Evidência de execução: `docs/evidence/EVIDENCE-template.md`
- Matriz de casos: `docs/test-matrix.md`
- Bugs documentados:
  - `docs/bugs/BUG-001-transacao-retorna-500-para-regra-negocio.md`
  - `docs/bugs/BUG-002-criacao-transacao-retorna-500.md`
  - `docs/bugs/BUG-003-encoding-corrompido-em-nome-na-resposta.md`
  - `docs/bugs/BUG-004-transacao-form-nao-filtra-categoria-por-tipo.md`

## Backend de testes (.NET + xUnit)

- Solução: `tests/backend/MinhasFinancas.BackendTests.sln`
- Unit tests: `tests/backend/unit/Backend.UnitTests`
- Integration tests: `tests/backend/integration/Backend.IntegrationTests`
- Casos unitários implementados: `RB-001`, `RB-004`, `RB-005`, `RB-006`

Comando de execução:

```bash
dotnet test tests/backend/MinhasFinancas.BackendTests.sln
```

## Frontend unit tests (Vitest)

- Projeto: `tests/frontend/unit`
- Configuração: `tests/frontend/unit/vitest.config.ts`
- Setup: `tests/frontend/unit/vitest.setup.ts`
- Casos implementados:
  - `RB-003` (passando)
  - `RB-008` (falhando, bug documentado)

Comandos:

```bash
npm --prefix tests/frontend/unit test
npm --prefix tests/frontend/unit run test:watch
npm --prefix tests/frontend/unit run test:coverage
```

## E2E tests (Playwright)

- Projeto: `tests/e2e`
- Configuração: `tests/e2e/playwright.config.ts`
- Variáveis de ambiente: `tests/e2e/.env.example`
- Casos implementados:
  - `RB-003` (passando)
  - `RB-008` (falhando, bug documentado)

Comandos:

```bash
npm --prefix tests/e2e test
npm --prefix tests/e2e run test:headed
npm --prefix tests/e2e run test:ui
npm --prefix tests/e2e run test:report
```

Com `BASE_WEB_URL` customizada:

```powershell
$env:BASE_WEB_URL="http://localhost:5175"; npm --prefix tests/e2e test
```

## Comandos consolidados (execução manual)

Backend:

```bash
dotnet test tests/backend/unit/Backend.UnitTests/Backend.UnitTests.csproj
dotnet test tests/backend/integration/Backend.IntegrationTests/Backend.IntegrationTests.csproj
```

Frontend:

```bash
npm --prefix tests/frontend/unit test
npm --prefix tests/e2e test
```

Execução recomendada (ordem):

1. Subir backend e frontend do sistema alvo.
2. Rodar backend unit.
3. Rodar backend integration.
4. Rodar frontend unit.
5. Rodar E2E.

## Estado atual da suíte

- **Passando**
  - `RB-001` (unit backend)
  - `RB-003` (frontend unit + e2e)
  - `RB-004` (unit backend)
  - `RB-005` (unit backend)
  - `RB-006` (unit backend)
  - `RB-011` (integration backend)
- **Falhando (defeitos encontrados)**
  - `RB-002` -> `BUG-001`
  - `RB-007` -> `BUG-001`
  - `RB-008` -> `BUG-004`
- **Bloqueado por defeito**
  - `RB-009` -> `BUG-002`
- **Ainda pendente**
  - `RB-010`
  - `RB-012`

## Bugs encontrados

- `BUG-001`: API retorna `500` para violações de regra que deveriam retornar `400`.
- `BUG-002`: criação de transação válida retorna `500`, bloqueando cenários dependentes.
- `BUG-003`: evidência de texto com acentuação corrompida na consulta de totais.
- `BUG-004`: frontend não filtra categorias por tipo de transação no formulário.

Detalhes completos em `docs/bugs`.

## Justificativa da pirâmide

- Unit tests no backend para validar regras de domínio/serviço de forma rápida e isolada.
- Integration tests no backend para validar contrato HTTP + persistência real.
- Frontend unit tests para validações de formulário e fluxo de UI com baixo custo.
- E2E para comprovar comportamento de ponta a ponta com evidência funcional.

## CI (GitHub Actions)

- Workflow em `.github/workflows/tests.yml`.
- Executa via `workflow_dispatch` para permitir informar a URL do repo A no momento da execução.
- Jobs padrão: backend unit, backend integration e frontend unit.
- Job E2E opcional (`run_e2e=true`).

Exemplo de uso no GitHub Actions (manual):

- `sut_repo`: URL Git do repositório da aplicação alvo.
- `run_e2e`: `true` quando quiser incluir Playwright.

## Checklist final de submissão

- [x] Repositório contém somente testes e documentação.
- [x] Pirâmide de testes implementada.
- [x] Regras principais cobertas com rastreabilidade (`docs/test-matrix.md`).
- [x] Bugs documentados em arquivos `.md`.
- [x] README com instruções de execução e estratégia.
- [x] Workflow de CI adicionado.

