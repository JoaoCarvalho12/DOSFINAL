using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantReservations.API.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public TimeSpan ReservationTime { get; set; }

        [Required]
        [Range(1, 50)]
        public int TableNumber { get; set; }

        [Required]
        [Range(1, 20)]
        public int NumberOfPeople { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}