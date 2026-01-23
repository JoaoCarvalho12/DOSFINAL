using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantReservations.API.Data;
using RestaurantReservations.API.DTOs;
using RestaurantReservations.API.Models;
using RestaurantReservations.API.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantReservations.UnitTests.Services
{
    public class ReservationServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            // Usar banco em memória para testes
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(_options);
            _service = new ReservationService(_context);
        }

        [Fact]
        public async Task CreateReservationAsync_Should_Create_Reservation()
        {
            // Arrange
            var dto = new CreateReservationDto
            {
                CustomerName = "Test Customer",
                ReservationDate = DateTime.Today.AddDays(1),
                ReservationTime = TimeSpan.FromHours(19),
                TableNumber = 1,
                NumberOfPeople = 4
            };

            // Act
            var result = await _service.CreateReservationAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Customer", result.CustomerName);
            Assert.Equal(1, result.TableNumber);
        }

        [Fact]
        public async Task CreateReservationAsync_Should_Throw_When_TimeConflict()
        {
            // Arrange
            var dto1 = new CreateReservationDto
            {
                CustomerName = "Customer 1",
                ReservationDate = DateTime.Today.AddDays(1),
                ReservationTime = TimeSpan.FromHours(19),
                TableNumber = 1,
                NumberOfPeople = 2
            };

            var dto2 = new CreateReservationDto
            {
                CustomerName = "Customer 2",
                ReservationDate = DateTime.Today.AddDays(1),
                ReservationTime = TimeSpan.FromHours(19), // Mesmo horário
                TableNumber = 1, // Mesma mesa
                NumberOfPeople = 4
            };

            // Act & Assert
            await _service.CreateReservationAsync(dto1);
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateReservationAsync(dto2)
            );
        }

        [Fact]
        public async Task GetAllReservationsAsync_Should_Return_All_Reservations()
        {
            // Arrange
            var dto = new CreateReservationDto
            {
                CustomerName = "Test",
                ReservationDate = DateTime.Today.AddDays(1),
                ReservationTime = TimeSpan.FromHours(12),
                TableNumber = 2,
                NumberOfPeople = 2
            };
            await _service.CreateReservationAsync(dto);

            // Act
            var result = await _service.GetAllReservationsAsync();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetReservationByIdAsync_Should_Return_Null_For_Invalid_Id()
        {
            // Act
            var result = await _service.GetReservationByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteReservationAsync_Should_Return_False_For_Invalid_Id()
        {
            // Act
            var result = await _service.DeleteReservationAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
public async Task UpdateReservationAsync_Should_Allow_Changing_NumberOfPeople_Only()
{
    // Arrange - Criar reserva inicial
    var createDto = new CreateReservationDto
    {
        CustomerName = "Test",
        ReservationDate = DateTime.Today.AddDays(1),
        ReservationTime = TimeSpan.FromHours(19),
        TableNumber = 1,
        NumberOfPeople = 4
    };
    
    var created = await _service.CreateReservationAsync(createDto);
    
    // Act - Atualizar apenas número de pessoas
    var updateDto = new UpdateReservationDto
    {
        NumberOfPeople = 6  // ← Apenas muda número de pessoas
    };
    
    var updated = await _service.UpdateReservationAsync(created.Id, updateDto);
    
    // Assert
    Assert.NotNull(updated);
    Assert.Equal(6, updated.NumberOfPeople);  // Novo valor
    Assert.Equal(1, updated.TableNumber);     // Mesa mantém
    Assert.Equal("Test", updated.CustomerName); // Nome mantém
}
    }
}