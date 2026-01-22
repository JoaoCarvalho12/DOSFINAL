using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantReservations.API.DTOs
{
    public class ReservationDto
    {
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
    }

    public class CreateReservationDto
    {
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
    }

    public class UpdateReservationDto
    {
        [MaxLength(100)]
        public string? CustomerName { get; set; }

        public DateTime? ReservationDate { get; set; }

        public TimeSpan? ReservationTime { get; set; }

        [Range(1, 50)]
        public int? TableNumber { get; set; }

        [Range(1, 20)]
        public int? NumberOfPeople { get; set; }
    }
}