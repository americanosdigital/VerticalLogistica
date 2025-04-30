# VerticalLogistica
# Vertical Logística - Desafio Técnico

Este é um projeto de integração para processamento de arquivos de pedidos, desenvolvido para o desafio técnico da Vertical Logística. O sistema recebe um arquivo de pedidos desnormalizado e o transforma em dados JSON normalizados, disponibilizando-os via API REST.

## Arquitetura

O projeto segue uma arquitetura em camadas com Design Orientado a Domínio (DDD) e princípios SOLID:

- **VerticalLogistica.Domain**: Contém as entidades, interfaces e filtros do domínio.
- **VerticalLogistica.Infrastructure**: Implementações das interfaces do domínio, como parsers e repositórios.
- **VerticalLogistica.Application**: Serviços de aplicação que orquestram as operações.
- **VerticalLogistica.API**: API REST para interação com o sistema.
- **VerticalLogistica.Tests**: Testes unitários e de integração.

## Tecnologias Utilizadas

- .NET 7.0
- ASP.NET Core Web API
- xUnit para testes
- Moq para mocks em testes
- FluentAssertions para assertivas nos testes
- Swagger para documentação da API

## Funcionalidades

- Upload de arquivo de pedidos via API REST
- Processamento e normalização dos dados
- Consulta de pedidos processados via API REST
- Filtragem por ID do pedido
- Filtragem por intervalo de data de compra

## Como Executar

### Pré-requisitos

- .NET 7.0 SDK ou superior

### Passos para Execução

1. Clone o repositório
2. Navegue até a pasta do projeto
3. Execute os comandos:

```bash
# Restaura as dependências
dotnet restore

# Compila o projeto
dotnet build

# Executa os testes
dotnet test

# Executa a API (a partir da pasta VerticalLogistica.API)
cd VerticalLogistica.API
dotnet run
```

A API estará disponível em `https://localhost:5001` ou `http://localhost:5000`.

### Endpoints da API

#### Consultar Pedidos

```
GET /api/orders
```

Parâmetros de consulta (opcionais):
- `orderId`: Filtrar por ID do pedido
- `startDate`: Data inicial no formato yyyy-MM-dd
- `endDate`: Data final no formato yyyy-MM-dd

#### Upload de Arquivo

```
POST /api/orders/upload
```

O arquivo deve ser enviado como form-data com a chave "file".

## Abordagem de Desenvolvimento

Este projeto foi desenvolvido seguindo a metodologia TDD (Test-Driven Development):

1. Escrever testes que definem o comportamento esperado
2. Implementar o código mínimo necessário para passar nos testes
3. Refatorar para melhorar a qualidade do código, mantendo os testes passando

##
