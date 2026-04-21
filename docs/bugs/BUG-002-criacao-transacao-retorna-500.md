# BUG-002 - Criação de transação válida retorna 500

## Metadados

- ID do bug: `BUG-002`
- Caso(s) relacionado(s): `RB-009`
- Regra de negócio impactada:
  - Exclusão em cascata de transações ao excluir pessoa (não pode ser validada porque a transação não é criada)
- Severidade: Crítica
- Ambiente:
  - API: `http://localhost:5135`
  - Web: não aplicável
  - Banco: SQLite (padrão do projeto)
  - Data/hora: 2026-04-21

## Descrição

Mesmo com dados válidos (pessoa maior + categoria finalidade ambas + transação despesa), o endpoint de criação de transação retorna erro interno.

## Pré-condições

- Pessoa maior de idade criada.
- Categoria com finalidade `Ambas` criada.

## Passos para reproduzir

1. Criar pessoa maior de idade (`POST /api/v1.0/Pessoas`).
2. Criar categoria finalidade `Ambas` (`POST /api/v1.0/Categorias`).
3. Criar transação compatível (`POST /api/v1.0/Transacoes`).

## Resultado esperado

Retornar `201 Created` com payload da transação criada.

## Resultado atual

Retorna `500 Internal Server Error` com mensagem de falha ao salvar alterações.

## Evidências

- Falha automatizada:
  - `RB009_Deve_Excluir_Transacoes_Ao_Excluir_Pessoa` (quebra no setup ao criar transação)
- Requisição manual:
  - `STATUS=500`
  - `BODY={"StatusCode":500,"Message":"Ocorreu um erro interno no servidor.","Detailed":"An error occurred while saving the entity changes. See the inner exception for details."}`

## Impacto

Fluxo central de cadastro de transações fica indisponível. Isso bloqueia validações de cascata, totais e demais funcionalidades dependentes.

## Observações adicionais

No startup da API há warnings de EF Core sobre chaves estrangeiras em shadow state para `CategoriaId1` e `PessoaId1`, o que pode estar relacionado ao erro de persistência.
