TESTES + BANCO DE DADOS

RESPONSABILIDADES:
- Criar testes unitários
- Configurar SQL Server
- Gerir migrações Entity Framework
- Garantir qualidade código

FICHEIROS:
- tests/ (todos os testes)
- src/RestaurantReservations.API/Data/
- docker-compose.yml (apenas SQL Server)
- RestaurantReservations.sln

COMANDOS:
cd tests/RestaurantReservations.UnitTests
dotnet test

INICIAR BD:
docker-compose up -d sqlserver

TESTES IMPLEMENTADOS:
Models/ReservationTests.cs (3 testes)
1. Reservation_Should_Have_Default_CreatedAt - Verifica data criação automática
2. Reservation_Should_Set_CustomerName("João Silva") - Testa atribuição nome
3. Reservation_Should_Set_CustomerName("Maria Santos") - Segundo caso de nome

DTOs/ReservationDtoTests.cs (2 testes)
4. CreateReservationDto_Validation_Should_Fail_When_Empty - Validação falha sem dados
5. CreateReservationDto_Validation_Should_Pass_When_Valid - Validação passa com dados válidos

Services/ReservationServiceTests.cs (6 testes)
6. CreateReservationAsync_Should_Create_Reservation - Cria reserva básica
7. CreateReservationAsync_Should_Throw_When_TimeConflict - CONFLITO HORÁRIO (teste crítico)
8. GetAllReservationsAsync_Should_Return_All_Reservations - Lista todas reservas
9. GetReservationByIdAsync_Should_Return_Null_For_Invalid_Id - ID inválido retorna null
10. DeleteReservationAsync_Should_Return_False_For_Invalid_Id - Delete falha com ID inválido
11. ReservationServiceTests (setup/constructor) - Configuração ambiente teste

Controllers/ReservationsControllerTests.cs
12. Post_Should_Return_Created_Reservation - Testa: POST /api/reservations → 201 Created - Verifica: API retorna status 201 e localização do recurso
13. Get_Should_Return_Ok_With_Reservations - Testa: GET /api/reservations → 200 OK - Verifica: API retorna lista de reservas
14. GetById_Should_Return_NotFound_For_Invalid_Id - Testa: GET /api/reservations/999 → 404 Not Found - Verifica: API responde corretamente para ID inválido