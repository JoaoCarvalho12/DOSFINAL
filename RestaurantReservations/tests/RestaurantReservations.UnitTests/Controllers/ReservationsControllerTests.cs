using Microsoft.AspNetCore.Mvc;
using Moq;
using RestaurantReservations.API.Controllers;
using RestaurantReservations.API.DTOs;
using RestaurantReservations.API.Services;
using Xunit;

namespace RestaurantReservations.UnitTests.Controllers
{
    public class ReservationsControllerTests
    {
        private readonly Mock<IReservationService> _mockService;
        private readonly ReservationsController _controller;

        public ReservationsControllerTests()
        {
            _mockService = new Mock<IReservationService>();
            _controller = new ReservationsController(_mockService.Object);
        }

        [Fact]
        public async Task Post_Should_Return_Created_Reservation()
        {
            // Arrange
            var createDto = new CreateReservationDto
            {
                CustomerName = "Cliente Teste",
                TableNumber = 1,
                ReservationDate = DateTime.Now.AddDays(1),
                ReservationTime = new TimeSpan(19, 0, 0),
                NumberOfPeople = 4
            };

            var expectedReservation = new ReservationDto
            {
                Id = 1,
                CustomerName = "Cliente Teste",
                TableNumber = 1,
                ReservationDate = DateTime.Now.AddDays(1),
                ReservationTime = new TimeSpan(19, 0, 0),
                NumberOfPeople = 4
            };

            _mockService.Setup(s => s.CreateReservationAsync(createDto))
                .ReturnsAsync(expectedReservation);

            // Act
            var actionResult = await _controller.CreateReservation(createDto);
            var createdAtResult = actionResult.Result as CreatedAtActionResult;

            // Assert
            Assert.NotNull(createdAtResult);
            Assert.Equal(201, createdAtResult.StatusCode);
            Assert.Equal(expectedReservation, createdAtResult.Value);
        }

        [Fact]
        public async Task Get_Should_Return_Ok_With_Reservations()
        {
            // Arrange
            var reservations = new List<ReservationDto>
            {
                new() { Id = 1, CustomerName = "Cliente Teste", TableNumber = 1 },
                new() { Id = 2, CustomerName = "Cliente Teste2", TableNumber = 2 }
            };

            _mockService.Setup(s => s.GetAllReservationsAsync())
                .ReturnsAsync(reservations);

            // Act
            var actionResult = await _controller.GetReservations();
            var okResult = actionResult.Result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(reservations, okResult.Value);
        }

        [Fact]
        public async Task GetById_Should_Return_NotFound_For_Invalid_Id()
        {
            // Arrange
            _mockService.Setup(s => s.GetReservationByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ReservationDto?)null);

            // Act
            var actionResult = await _controller.GetReservation(999);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}