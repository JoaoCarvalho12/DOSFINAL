using Microsoft.EntityFrameworkCore;
using RestaurantReservations.API.Models;

namespace RestaurantReservations.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de índices para melhor performance nas consultas
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.ReservationDate, r.TableNumber, r.ReservationTime })
                .IsUnique()
                .HasDatabaseName("IX_Reservation_Date_Table_Time");

            // Configuração de validações adicionais
            modelBuilder.Entity<Reservation>()
                .Property(r => r.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.TableNumber)
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.NumberOfPeople)
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}