# Motor de Rebalanceamento

O motor de rebalanceamento é executado nas seguintes situações abaixo:

#### A) Mundaça na composição da cesta

Quando o administrador altera a cesta de recomendação:
1. Identificar os ativos que **saíram** da cesta
2. Para cada cliente, **vender** a posição dos ativos que saíram
3. Com o valor obtido, **comprar** os novos ativos segundo a nova composição
4. Atualizar a custodia filhote de cada cliente

#### B) Rebalanceamento por desvio de proporção

Quando a valorização ou desvalorização de um ativo causa um desvio significativo na proporção da carteira do cliente em relação a cesta recomendada:
1. Calcular a proporção atual de cada ativo na carteira do cliente
2. Comparar com os percentuais da cesta recomendada
3. Vender ativos que estão acima da proporção alvo
4. Comprar ativos que estão abaixo da proporção alvo

#### Regras de IR no rebalanceamento:

- Somar todas as vendas do cliente no mês corrente
- Se o total de vendas **exceder R$ 20.000,00**, calcular **20% de IR sobre o lucro liquido** (diferença entre valor de venda e custo de aquisição pelo preço médio)
- Publicar o valor do IR no topico Kafka