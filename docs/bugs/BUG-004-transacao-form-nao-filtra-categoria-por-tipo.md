# BUG-004 - TransacaoForm nao repassa tipo para filtro de categoria

## Metadados

- ID do bug: `BUG-004`
- Caso(s) relacionado(s): `RB-008`
- Regra de negócio impactada:
  - Categoria só pode ser usada conforme finalidade (receita/despesa/ambas)
- Severidade: Alta
- Ambiente:
  - Web: React/TypeScript
  - Data/hora: 2026-04-21

## Descrição

No formulário de transação, o componente de categoria não recebe o tipo selecionado (`despesa`/`receita`) para aplicar filtro de categorias compatíveis.

## Pré-condições

- Renderizar `TransacaoForm`.

## Passos para reproduzir

1. Selecionar `Tipo = receita` no formulário.
2. Observar propriedades enviadas para `LazyCategoriaSelect`.

## Resultado esperado

`LazyCategoriaSelect` deve receber `selectedTipo` com o valor atual do tipo para filtrar categorias compatíveis.

## Resultado atual

`selectedTipo` é `undefined`, então o filtro por finalidade não é aplicado no componente.

## Evidências

- Teste automatizado falhando:
  - `RB-008 deve repassar o tipo selecionado para filtro de categorias`
- Teste E2E falhando:
  - `RB-008 deve impedir salvar transacao com categoria incompativel`
  - Resultado observado: opção `Alimentação` (finalidade despesa) aparece mesmo com `Tipo=receita`
- Localização observada:
  - `TransacaoForm` renderiza `LazyCategoriaSelect` sem prop `selectedTipo`.

## Impacto

Usuário pode selecionar categoria incompatível com o tipo de transação, delegando validação apenas para backend e degradando UX.

## Observações adicionais

O backend já rejeita inconsistências em domínio, mas a regra deveria ser reforçada no frontend para evitar tentativa inválida.
