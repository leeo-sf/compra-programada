# Interface do Cliente (API REST)

| Endpoint | Descricao |
|---|---|
| **/api/clientes/adesao** | Cliente adere ao produto informando seus dados e o valor mensal de aporte. O sistema cria a conta grafica e custodia filhote |
| **/api/clientes/{id}/saida** | Cliente solicita saida do produto. O sistema interrompe novas compras, mas **mantem a posição existente** na custodia filhote |
| **/api/clientes/{id}/valor-mensal** | Cliente altera o valor do aporte mensal (Valor refletido na próxima compra) |
| **/api/clientes/{id}/carteira** | Cliente visualiza sua custodia: ativos, quantidades, preço médio, valor atual, P/L (lucro/prejuizo) e rentabilidade |
| **/api/clientes/{id}/rentabilidade** | Exibe informações detalhadas de rentabilidade da carteira do cliente, incluindo: saldo total, P/L por ativo, P/L total, rentabilidade percentual, historico de evolucao, e demais informacoes pertinentes ao acompanhamento de rentabilidade de uma carteira em corretora |

## Painel Administrativo (API REST)

| Endpoint | Descricao |
|---|---|
| **/api/admin/cesta** | Registra/Altera as 5 ações recomendadas e o percentual de cada uma na cesta (a soma dos percentuais deve ser 100%) |
| **/api/admin/cesta/atual** | Retorna a composição atual da cesta de recomendação |
| **/api/admin/cesta/historico** | Retorna o historico de alterações da cesta |