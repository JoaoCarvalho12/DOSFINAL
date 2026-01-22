using RestaurantReservations.API.Models;

namespace RestaurantReservations.UnitTests.Models
{
    public class ReservationTests
    {
        [Fact]
        public void Reservation_Should_Have_Default_CreatedAt()
        {
            // Arrange
            var reservation = new Reservation();

            // Act & Assert
            Assert.True(reservation.CreatedAt <= DateTime.UtcNow);
            Assert.True(reservation.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
        }

        [Theory]
        [InlineData("João Silva")]
        [InlineData("Maria Santos")]
        public void Reservation_Should_Set_CustomerName(string customerName)
        {
            // Arrange & Act
            var reservation = new Reservation
            {
                CustomerName = customerName
            };

            // Assert
            Assert.Equal(customerName, reservation.CustomerName);
        }
    }
}