using Microsoft.AspNetCore.Mvc;
using Moq;
using RestaurantReservations.API.Controllers;
using RestaurantReservations.API.DTOs;
using RestaurantReservations.API.Services;
using System;
using System.Threading.Tasks;
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
            var dto = new CreateReservationDto
            {
                CustomerName = "Test Customer",
                ReservationDate = DateTime.Today.AddDays(1),
                ReservationTime = TimeSpan.FromHours(19),
                TableNumber = 1,
                NumberOfPeople = 2
            };

            var expectedReservation = new ReservationDto
            {
                Id = 1,
                CustomerName = "Test Customer",
                ReservationDate = DateTime.Today.AddDays(1),
                ReservationTime = TimeSpan.FromHours(19),
                TableNumber = 1,
                NumberOfPeople = 2
            };

            _mockService.Setup(s => s.CreateReservationAsync(dto))
                .ReturnsAsync(expectedReservation);

            // Act
            var result = await _controller.CreateReservation(dto) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            var returnValue = Assert.IsType<ReservationDto>(result.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Get_Should_Return_Ok_With_Reservations()
        {
            // Arrange
            var reservations = Array.Empty<ReservationDto>();
            _mockService.Setup(s => s.GetAllReservationsAsync())
                .ReturnsAsync(reservations);

            // Act
            var result = await _controller.GetAllReservations(null) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservations, result.Value);
        }

        [Fact]
        public async Task GetById_Should_Return_NotFound_For_Invalid_Id()
        {
            // Arrange
            _mockService.Setup(s => s.GetReservationByIdAsync(999))
                .ReturnsAsync((ReservationDto?)null);

            // Act
            var result = await _controller.GetReservation(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}