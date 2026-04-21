# BUG-001 - Transação retorna 500 em violação de regra de negócio

## Metadados

- ID do bug: `BUG-001`
- Caso(s) relacionado(s): `RB-002`, `RB-007`
- Regra de negócio impactada:
  - Menor de idade não pode ter receitas
  - Categoria deve respeitar finalidade
- Severidade: Alta
- Ambiente:
  - API: `http://localhost:5135`
  - Web: não aplicável
  - Banco: SQLite (padrão do projeto)
  - Data/hora: 2026-04-21

## Descrição

Quando uma regra de negócio de transação é violada, a API responde `500 Internal Server Error` em vez de `400 Bad Request`.

## Pré-condições

- Backend em execução.
- Pessoa e categoria existentes para o cenário.

## Passos para reproduzir

1. Criar pessoa menor de idade.
2. Criar categoria com finalidade receita.
3. Tentar criar transação do tipo receita para a pessoa menor.
4. Repetir com pessoa maior + categoria receita + transação do tipo despesa.

## Resultado esperado

A API deve retornar `400 Bad Request` com mensagem de validação/regra de negócio.

## Resultado atual

A API retorna `500 Internal Server Error`.

## Evidências

- Falhas automatizadas:
  - `RB002_Deve_Bloquear_Receita_Para_Menor_Via_API`
  - `RB007_Deve_Bloquear_Categoria_Incompativel_Via_API`
- Resposta observada:
  - `StatusCode: 500`

## Impacto

Regras de negócio até parecem existir, mas o contrato HTTP está incorreto para erro de domínio, prejudicando consumidores da API e UI.

## Observações adicionais

O `TransacoesController` trata `ArgumentException` em `Create`, porém as regras de domínio lançam `InvalidOperationException`, o que não é convertido para `400`.
