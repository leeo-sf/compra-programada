# Motor de Compra Programada

O motor de compra é o coracao do sistema. Ele deve executar o seguinte fluxo nos **dias úteis iguais ou subsequentes ao dia 5, 15 e 25 de cada mês** (considerando dias úteis como segunda a sexta-feira).

Por exemplo: Hoje é dia 25/04/2024 (Sábado), o sistema não executará a compra devido não ser um dia útil. A compra será executada no dia 27/04/2026 (Segunda).

#### Passo a passo do motor:

1. **Agrupamento de pedidos:** Coleta todos os clientes ativos e calcular **1/3 do valor mensal** de cada cliente para a data corrente (o valor mensal é dividido em 3 parcelas: dia 5, dia 15 e dia 25)

2. **Cálculo da compra consolidada:** Soma os valores de todos os clientes e calcula a quantidade de cada ativo a comprar segundo os percentuais da cesta vigente, utilizando a **cotação de fechamento** do último pregão disponível no arquivo COTAHIST.

3. **Consideração do saldo da custodia master:** Antes de emitir a ordem de compra, verifica se há saldo remanescente na custodia master para cada ativo. Se houver, desconta do total a comprar.

4. **Execução da compra:** Registra a compra na conta master (priorizar lotes padrão e útiliza mercado fracionário para o restante).

5. **Distribuição para contas filhotes:** Distribuí os ativos comprados para cada custodia filhote proporcionalmente ao valor do aporte de cada cliente.

6. **Resíduos:** Caso após a distribuição total ainda existam ativos remanescentes na conta master (por arredondamentos ou frações), esses ativos devem ser mantidos na custodia master para serem considerados na próxima data de compra.

7. **IR dedo-duro:** Para cada distribuição ao cliente, calcula o IR dedo-duro (0,005% sobre o valor da operacao) e publica em um **topico Kafka** com as informações necessárias para eventual envio a Receita Federal.