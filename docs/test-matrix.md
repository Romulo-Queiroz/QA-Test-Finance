# Matriz de Casos de Teste

Esta matriz mapeia as regras de negócio prioritárias do sistema para cenários de teste organizados na pirâmide.

## Convenções

- **Nível**: Unit, Integration, E2E
- **Status inicial**: Planned
- **Resultado esperado**: comportamento correto segundo o enunciado

## Casos Prioritários

| ID | Regra | Cenário | Nível | Resultado Esperado | Status |
|---|---|---|---|---|---|
| RB-001 | Menor de idade não pode ter receitas | Criar transação do tipo receita para pessoa com menos de 18 anos | Unit | Validação deve rejeitar operação com erro de regra de negócio | Implemented |
| RB-002 | Menor de idade não pode ter receitas | Tentar criar receita para menor via endpoint de transação | Integration | API deve retornar erro de validação e não persistir transação | Implemented (Failing - BUG-001) |
| RB-003 | Menor de idade não pode ter receitas | Usuário preenche formulário de transação para menor e seleciona receita | Frontend Unit + E2E | Fluxo deve bloquear confirmação e exibir mensagem de erro | Implemented (Passing) |
| RB-004 | Categoria respeita finalidade | Criar despesa usando categoria somente receita | Unit | Validação deve rejeitar associação categoria/tipo incompatível | Implemented |
| RB-005 | Categoria respeita finalidade | Criar receita usando categoria somente despesa | Unit | Validação deve rejeitar associação categoria/tipo incompatível | Implemented |
| RB-006 | Categoria respeita finalidade | Criar transação com categoria de finalidade ambas | Unit | Validação deve permitir associação válida | Implemented |
| RB-007 | Categoria respeita finalidade | Criar transação incompatível via endpoint | Integration | API deve rejeitar e não persistir registro inválido | Implemented (Failing - BUG-001) |
| RB-008 | Categoria respeita finalidade | Selecionar categoria incompatível no fluxo de cadastro de transação | Frontend Unit + E2E | Fluxo deve impedir salvamento e sinalizar erro ao usuário | Implemented (Failing - BUG-004) |
| RB-009 | Exclusão em cascata de transações | Excluir pessoa que possui transações associadas | Integration | Pessoa e transações vinculadas devem ser removidas corretamente | Blocked by BUG-002 |
| RB-010 | Exclusão em cascata de transações | Excluir pessoa pela UI e consultar lista de transações | E2E | Transações da pessoa excluída não devem mais aparecer | Planned |
| RB-011 | Totais por pessoa | Consultar total por pessoa após cadastrar receita e despesa válidas | Integration | Total deve refletir soma correta conforme regras do sistema | Implemented (Passing) |
| RB-012 | Totais por pessoa | Validar atualização de totais no dashboard após operações | E2E | Indicadores devem refletir dados persistidos mais recentes | Planned |

## Critérios de Priorização

- Cobrir primeiro as regras centrais do enunciado (RB-001 a RB-010).
- Em seguida, validar consistência de totais (RB-011 e RB-012).
- Cada bug encontrado deve referenciar o ID da regra/caso desta matriz.

## Rastreabilidade para Bugs

Ao documentar um bug em `docs/bugs`, usar o formato:

- Caso relacionado: `RB-xxx`
- Regra impactada: descrição curta
- Evidência: prints/logs/resultado do teste
- Esperado vs atual
