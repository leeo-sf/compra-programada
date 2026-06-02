# Compra Programada

A `Compra Programada` é uma aplicação para clientes que realizam investimento. O produto permite que clientes adiram a um plano de investimento recorrente e automatizado em uma carteira recomendada de 5 ações, definida pelo administrador do sistema (ou determinada equipe).

O cliente escolhe um valor mensal de aporte, e o sistema automaticamente:
- Executa as compras de ações de forma consolidada (na conta da corretora).
- Distribui os ativos proporcionalmente para a custodia individual de cada cliente.
- Gerencia rebalanceamentos quando a composicao da carteira recomendada muda ou quando ha desvios significativos de proporcao *(Funcionalidade em breve)*.

#### Para mais documentações do projeto acesse os links abaixo:
- [Motor de Compra](https://github.com/leeo-sf/compra-programada/blob/main/docs/motor-compra.md)
- [Motor de Rebalanceamento](https://github.com/leeo-sf/compra-programada/blob/main/docs/motor-rebalanceamento.md)
- [Funcionalidades](https://github.com/leeo-sf/compra-programada/blob/main/docs/funcionalidades.md)

## Conceitos Importantes do Mercado Financeiro

É importante saber alguns conceitos para entender o funcionamento do sistema.

### 1.0. Lote Padrão VS. Mercado Fracionário

Na B3 (Bolsa de Valores do Brasil), as ações podem ser negociadas de duas formas:

- **Lote Padrão (Mercado Primário):** As ações são negociadas em lotes de **100 unidades**. Por exemplo, para comprar ações da PETR4 no mercado de lote padrão, voce deve comprar multiplos de 100 (100, 200, 300...). O ticker utilizado é o código padrão do ativo (ex: `PETR4`).

- **Mercado Fracionário:** Permite a compra de **1 a 99 unidades** de uma ação. O ticker no mercado fracionário recebe o sufixo `F` (ex: `PETR4F`). Isso possibilita investimentos com valores menores, pois não é necessário comprar o lote inteiro.

### 1.1. IR "Dedo-Duro" (Imposto de Renda Retido na Fonte)

O "dedo-duro" é o apelido para o **Imposto de Renda Retido na Fonte (IRRF)** que incide sobre operações de renda variável. A aliquota é de **0,005%** sobre o valor total da operação de venda.

Este imposto é retido automaticamente pela corretora e serve como um mecanismo de rastreamento da Receita Federal para identificar operações realizadas pelo investidor. O valor retido pode ser descontado do IR devido na apuração mensal.

### 1.2. Isenção de IR para Pessoa Física em Vendas de Ações

Pessoas físicas são **isentas** de Imposto de Renda sobre o lucro de vendas de ações quando o **total de vendas no mês não ultrapassa R$ 20.000,00**.

- Se o total de vendas no mês **exceder R$ 20.000,00**, incide **20% de imposto sobre o lucro líquido** de todas as vendas do mês.
- O lucro e calculado como: `Valor de Venda - (Quantidade * Preço médio de Aquisição)`

### 1.4. Preço Médio de Aquisição

O preço médio de aquisição é o custo médio ponderado de compra de um ativo por um investidor. Ele é fundamental para:

- Calcular o **lucro ou prejuizo** em vendas (para fins de IR)
- Acompanhar a **rentabilidade** da carteira

**Formula:**

```
preço médio = (Quantidade Anterior * preço médio Anterior + Quantidade Nova * preço Nova Compra) / (Quantidade Anterior + Quantidade Nova)
```

### 1.4. Arquivo COTAHIST da B3

A B3 disponibiliza diariamente arquivos com as cotações historicas de todos os ativos negociados, chamado **COTAHIST**. Este arquivo contem informações como:

- Código do ativo
- Data do pregão
- Preço de abertura, fechamento, máximo e minímo
- Volume negociado

**Como obter o arquivo:**

1. Acesse o site da B3 [aqui](https://www.b3.com.br/pt_br/market-data-e-indices/servicos-de-dados/market-data/historico/mercado-a-vista/cotações-historicas/)
2. Selecione o periodo desejado (Diário, Mensal ou Anual)
3. Faça o download do arquivo no formato TXT
4. O layout do arquivo segue a especificação documentada pela B3 (disponível no mesmo site)


## 💻​ Caracteristicas do projeto:

- .NET 10
- ORM: Entity Framework
- Framework de Testes: xUnit
- Framework de Assertions: FluentAssertions e xUnit
- Framework de Mock: NSubstitute
- Cobertura de testes mínima: 70%
- Banco de dados: PostgreSQL
- Minimal API´s
- Mensageria: Kafka
- Padrões de projetos: SOLID, Clean Architecture, DDD
- Propagação de header `X-Request-Id` para rastreamento
- Log com Serilog
- Docker & Docker compose


### Requisitos de Qualidade

- **Cobertura de testes:** Minimo de **70%** de cobertura de testes unitários e/ou de integração.
- **Código limpo:** Seguimento de boas práticas de Clean Code, SOLID e design patterns adequados.

## Estrutura do Projeto

```
/
|-- .github/                   # Pasta com os arquivo de pipeline
|-- cotações/                  # Pasta com arquivos COTAHIST da B3
|   |-- COTAHIST_D20260225.TXT
|   |-- COTAHIST_D20260226.TXT
|   +-- ...
|-- docker/                    # Pasta com o arquivo dockerfile e docker compose
|-- src/                       # Código-fonte do sistema
|-- tests/                     # Testes unitarios e de integração
|-- .runsettings               # Arquivo de configuração do coverage (cobertura de testes)
|-- README.md                  # Documentacao do projeto
|-- VERSION.targets            # Arquivo para gerenciamento de versões da aplicação
+-- ...
```

## 📃 Setup

- Ferramentas necessárias:
    .NET Core SDK 10.0, Docker, VS Code ou Visual Studio (Opcional).
- **Em breve: Executar o projeto local**