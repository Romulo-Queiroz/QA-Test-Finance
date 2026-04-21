# BUG-003 - Encoding corrompido em nome na resposta de totais por pessoa

## Metadados

- ID do bug: `BUG-003`
- Caso(s) relacionado(s): `RB-011`
- Regra de negócio impactada:
  - Integridade de dados exibidos nas consultas de totais
- Severidade: Média
- Ambiente:
  - API: `http://localhost:5135`
  - Web: não aplicável
  - Banco: SQLite (padrão do projeto)
  - Data/hora: 2026-04-21

## Descrição

A resposta do endpoint de totais por pessoa retorna nome com acentuação corrompida (ex.: `João` exibido como `Jo�o`).

## Pré-condições

- Backend em execução com seed inicial aplicado.

## Passos para reproduzir

1. Executar `GET /api/v1.0/Totais/pessoas?page=1&pageSize=20`.
2. Inspecionar o campo `nome` dos itens retornados.

## Resultado esperado

Os dados textuais devem ser retornados em UTF-8 correto, preservando acentos (`João Silva`).

## Resultado atual

O nome é retornado corrompido (`Jo�o Silva`).

## Evidências

- Resposta observada:
  - `{"nome":"Jo�o Silva","totalReceitas":3000.0,"totalDespesas":150.0,...}`

## Impacto

Indica risco de problema de encoding e degrada a confiabilidade da informação para usuários em português.

## Observações adicionais

O defeito deve ser tratado como crítico de integridade textual/UTF-8 no fluxo de serialização ou origem dos dados.
