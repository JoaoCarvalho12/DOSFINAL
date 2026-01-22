PROJETO: RESTAURANT RESERVATIONS API

DESCRIÇÃO:
API REST para gestão de reservas de mesas em restaurantes.

TECNOLOGIAS:
- ASP.NET Core 9.0
- SQL Server 2022
- Entity Framework Core
- Docker & Kubernetes
- Jenkins CI/CD
- SonarQube

COMANDOS:

1. INICIAR SQL SERVER:
   docker-compose up -d sqlserver

2. EXECUTAR API:
   cd src/RestaurantReservations.API
   dotnet run

3. EXECUTAR TESTES:
   cd tests/RestaurantReservations.UnitTests
   dotnet test

4. ACESSAR SWAGGER:
   http://localhost:5188/swagger ==> Ambiente de Desenvolvimento
   http://localhost:5000/swagger ==> Ambiente de Produção

ESTRUTURA:
- src/          - Código da API
- tests/        - Testes unitários  
- helm/         - Kubernetes Helm
- docker-compose.yml - Containers
- Jenkinsfile   - Pipeline CI/CD

GRUPO: 3 PESSOAS
1. API REST
2. Testes + Banco de Dados
3. DevOps & Infraestrutura