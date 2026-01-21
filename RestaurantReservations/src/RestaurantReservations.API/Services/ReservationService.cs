using Microsoft.EntityFrameworkCore;
using RestaurantReservations.API.Data;
using RestaurantReservations.API.DTOs;
using RestaurantReservations.API.Models;
using System;

namespace RestaurantReservations.API.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
        Task<ReservationDto?> GetReservationByIdAsync(int id);
        Task<IEnumerable<ReservationDto>> GetReservationsByDateAsync(DateTime date);
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task<ReservationDto?> UpdateReservationAsync(int id, UpdateReservationDto dto);
        Task<bool> DeleteReservationAsync(int id);
        Task<bool> HasTimeConflictAsync(int tableNumber, DateTime date, TimeSpan time, int? excludeReservationId = null);
    }

    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
        {
            var reservations = await _context.Reservations.ToListAsync();
            return reservations.Select(MapToDto);
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            return reservation == null ? null : MapToDto(reservation);
        }

        public async Task<IEnumerable<ReservationDto>> GetReservationsByDateAsync(DateTime date)
        {
            var reservations = await _context.Reservations
                .Where(r => r.ReservationDate.Date == date.Date)
                .ToListAsync();
            
            return reservations.Select(MapToDto);
        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            // VALIDAÇÃO 1: Não permitir reservas no passado
            ValidateFutureReservation(dto.ReservationDate, dto.ReservationTime);

            // VALIDAÇÃO 2: Verificar conflito de horário
            if (await HasTimeConflictAsync(dto.TableNumber, dto.ReservationDate, dto.ReservationTime))
            {
                throw new InvalidOperationException($"Mesa {dto.TableNumber} já está reservada para {dto.ReservationDate:d} às {dto.ReservationTime:hh\\:mm}");
            }

            var reservation = new Reservation
            {
                CustomerName = dto.CustomerName,
                ReservationDate = dto.ReservationDate,
                ReservationTime = dto.ReservationTime,
                TableNumber = dto.TableNumber,
                NumberOfPeople = dto.NumberOfPeople,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return MapToDto(reservation);
        }

        public async Task<ReservationDto?> UpdateReservationAsync(int id, UpdateReservationDto dto)
        {
            // 1. Primeiro encontrar a reserva
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return null;

            // 2. Calcular novos valores (ou manter os antigos)
            var newDate = dto.ReservationDate ?? reservation.ReservationDate;
            var newTime = dto.ReservationTime ?? reservation.ReservationTime;
            var newTable = dto.TableNumber ?? reservation.TableNumber;

            // 3. VALIDAÇÃO: Não permitir atualizar para data/hora no passado
            if (dto.ReservationDate.HasValue || dto.ReservationTime.HasValue)
            {
                ValidateFutureReservation(newDate, newTime);
            }

            // 4. Verificar se está mudando dados críticos
            var isChangingCriticalInfo = 
                newTable != reservation.TableNumber || 
                newDate != reservation.ReservationDate || 
                newTime != reservation.ReservationTime;

            // 5. Só verificar conflito se estiver mudando mesa, data ou hora
            if (isChangingCriticalInfo && await HasTimeConflictAsync(newTable, newDate, newTime, id))
            {
                throw new InvalidOperationException($"Mesa {newTable} já está reservada para {newDate:d} às {newTime:hh\\:mm}");
            }

            // 6. Atualizar propriedades se fornecidas
            if (dto.CustomerName != null) reservation.CustomerName = dto.CustomerName;
            if (dto.ReservationDate.HasValue) reservation.ReservationDate = dto.ReservationDate.Value;
            if (dto.ReservationTime.HasValue) reservation.ReservationTime = dto.ReservationTime.Value;
            if (dto.TableNumber.HasValue) reservation.TableNumber = dto.TableNumber.Value;
            if (dto.NumberOfPeople.HasValue) reservation.NumberOfPeople = dto.NumberOfPeople.Value;

            await _context.SaveChangesAsync();
            return MapToDto(reservation);
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasTimeConflictAsync(int tableNumber, DateTime date, TimeSpan time, int? excludeReservationId = null)
        {
            var query = _context.Reservations
                .Where(r => r.TableNumber == tableNumber &&
                           r.ReservationDate.Date == date.Date &&
                           r.ReservationTime == time);

            if (excludeReservationId.HasValue)
            {
                query = query.Where(r => r.Id != excludeReservationId.Value);
            }

            return await query.AnyAsync();
        }

        // MÉTODO PRIVADO PARA VALIDAR DATAS FUTURAS
        private void ValidateFutureReservation(DateTime date, TimeSpan time)
        {
            var reservationDateTime = new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hours,
                time.Minutes,
                0);

            var currentDateTime = DateTime.Now;

            if (reservationDateTime <= currentDateTime)
            {
                throw new ArgumentException("A data e hora da reserva devem ser no futuro");
            }
        }

        private ReservationDto MapToDto(Reservation reservation)
        {
            return new ReservationDto
            {
                Id = reservation.Id,
                CustomerName = reservation.CustomerName,
                ReservationDate = reservation.ReservationDate,
                ReservationTime = reservation.ReservationTime,
                TableNumber = reservation.TableNumber,
                NumberOfPeople = reservation.NumberOfPeople
            };
        }
    }
}