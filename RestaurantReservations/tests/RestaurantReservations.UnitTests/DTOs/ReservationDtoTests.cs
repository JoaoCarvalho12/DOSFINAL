using RestaurantReservations.API.DTOs;
using System.ComponentModel.DataAnnotations;

namespace RestaurantReservations.UnitTests.DTOs
{
    public class ReservationDtoTests
    {
        [Fact]
        public void CreateReservationDto_Validation_Should_Fail_When_Empty()
        {
            // Arrange
            var dto = new CreateReservationDto();
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("CustomerName"));
        }

        [Fact]
        public void CreateReservationDto_Validation_Should_Pass_When_Valid()
        {
            // Arrange
            var dto = new CreateReservationDto
            {
                CustomerName = "João Silva",
                ReservationDate = DateTime.Today.AddDays(1),
                ReservationTime = TimeSpan.FromHours(19),
                TableNumber = 5,
                NumberOfPeople = 4
            };
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
        }
    }
}