# VerticalLogistica

🧱 Arquitetura
O projeto segue uma arquitetura em camadas, baseada em Design Orientado a Domínio (DDD) e princípios SOLID:

VerticalLogistica.Domain: Contém as entidades, interfaces e filtros do domínio.

VerticalLogistica.Infrastructure: Implementações das interfaces do domínio, como parsers e repositórios.

VerticalLogistica.Application: Serviços de aplicação que orquestram as operações.

VerticalLogistica.API: API REST para interação com o sistema.

VerticalLogistica.Tests: Testes unitários e de integração com TDD.

🛠️ Tecnologias Utilizadas
.NET 7.0

ASP.NET Core Web API

xUnit para testes

Moq para mocks em testes

FluentAssertions para assertivas nos testes

Swagger para documentação da API

✨ Funcionalidades
Upload de arquivo de pedidos no formato legado via API REST

Processamento e normalização dos dados para formato JSON

Consulta de pedidos via API REST

Filtragem por:

ID do pedido (orderId)

Intervalo de datas de compra (startDate e endDate)

🚀 Como Executar
Pré-requisitos
.NET SDK 7.0 ou superior

Passos para execução:
# 1. Clonar o repositório
git clone https://github.com/seuusuario/VerticalLogistica.git
cd VerticalLogistica

# 2. Restaurar dependências
dotnet restore

# 3. Compilar a solução
dotnet build

# 4. Executar os testes
dotnet test

# 5. Rodar a API
cd VerticalLogistica.API

dotnet run

A API estará disponível em https://localhost:7183 ou http://localhost:5092

🔄 Automação (Build e Coverage)

🧪 Execução de Testes com Cobertura

Instale o pacote coverlet.collector no projeto de testes:

dotnet add package coverlet.collector
dotnet add package coverlet.collector
Execute os testes com coleta de cobertura:

bash
Copy
Edit
dotnet test --collect:"XPlat Code Coverage"
Para visualizar o resultado da cobertura no formato HTML, instale o reportgenerator:

bash
Copy
Edit
dotnet tool install -g dotnet-reportgenerator-globaltool
Gere o relatório HTML:

bash
Copy
Edit
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
Acesse o relatório gerado em coveragereport/index.html.

⚙️ Automação com GitHub Actions (Exemplo)
Crie o arquivo .github/workflows/build.yml:

yaml
Copy
Edit
name: Build and Test

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 7.0.x

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test with coverage
        run: dotnet test --no-build --collect:"XPlat Code Coverage"

🗺️ Desenho da API
POST /api/orders/upload
Descrição: Realiza o upload de um arquivo de pedidos legados para processamento.

Formato: multipart/form-data

Campo: file (arquivo .txt)

Respostas:

200 OK: Arquivo processado com sucesso.

400 BadRequest: Nenhum arquivo enviado ou inválido.

GET /api/orders
Descrição: Retorna a lista de usuários com seus pedidos normalizados.

Parâmetros de query (todos opcionais):

orderId: Filtrar por código do pedido

startDate: Data inicial (yyyy-MM-dd)

endDate: Data final (yyyy-MM-dd)

Respostas:

200 OK: Lista de objetos User, cada um contendo seus pedidos e produtos.

Exemplo de resposta:

json
Copy
Edit
[
  {
    "userId": 1,
    "name": "Zarelli",
    "orders": [
      {
        "orderId": 123,
        "purchaseDate": "2021-12-01T00:00:00",
        "total": 1024.48,
        "products": [
          { "productId": 111, "value": 512.24 },
          { "productId": 122, "value": 512.24 }
        ]
      }
    ]
  }
]

✅ Abordagem de Desenvolvimento
Este projeto foi desenvolvido com foco em TDD (Test-Driven Development):

Escrever testes unitários e de integração primeiro.

Implementar a lógica mínima necessária.

Refatorar com segurança mantendo os testes verdes.