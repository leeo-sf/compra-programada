# Compra.Programada

A `Compra.Programada` é uma aplicação para clientes que realizam investimento. O produto permite que clientes adiram a um plano de investimento recorrente e automatizado em uma carteira recomendada de 5 ações, definida pelo administrador do sistema (ou determinada equipe).

O cliente escolhe um valor mensal de aporte, e o sistema automaticamente:
- Executa as compras de ações de forma consolidada (na conta da corretora).
- Distribui os ativos proporcionalmente para a custodia individual de cada cliente.
- Gerencia rebalanceamentos quando a composicao da carteira recomendada muda ou quando ha desvios significativos de proporcao *(Funcionalidade em breve)*.


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

## 📃 Setup

- Ferramentas necessárias:
    Visual Studio ou VS Code, .NET Core SDK 10.0, Banco de dados postgre configurado com schema do projeto.
- Executar o projeto `src/CompraProgramada.Api`