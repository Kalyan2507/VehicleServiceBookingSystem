using Xunit;

using Moq;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

using System.Collections.Generic;

using System.Linq;

using VehicleServiceBook.Models.Domains;

using VehicleServiceBook.Repositories;



namespace VehicleServiceBook.Tests

{

    public class BookingRepositoryTests

    {

        private readonly BookingRepository _repo;

        private readonly VehicleServiceBookContext _context;



        public BookingRepositoryTests()

        {

            // Use in-memory database

            var options = new DbContextOptionsBuilder<VehicleServiceBookContext>()

        .UseInMemoryDatabase(databaseName: "TestDb")

        .Options;



            _context = new VehicleServiceBookContext(options);

            _repo = new BookingRepository(_context);



            // Seed data

            _context.Bookings.Add(new Booking { Bookingid = 1, UserId = 100 });

            _context.Bookings.Add(new Booking { Bookingid = 2, UserId = 200 });

            _context.SaveChanges();

        }



        [Fact]

        public async Task GetByIdAsync_ReturnsCorrectBooking()

        {

            var booking = await _repo.GetByIdAsync(1);

            Assert.NotNull(booking);

            Assert.Equal(100, booking.UserId);

        }



        [Fact]

        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()

        {

            var booking = await _repo.GetByIdAsync(999);

            Assert.Null(booking);

        }



        [Fact]

        public async Task GetAllByUserIdAsync_ReturnsCorrectBookings()

        {

            var bookings = await _repo.GetAllByUserIdAsync(100);

            Assert.Single(bookings);

        }



        [Fact]

        public async Task AddAsync_AddsBooking()

        {

            var newBooking = new Booking { Bookingid = 3, UserId = 300 };

            await _repo.AddAsync(newBooking);

            await _repo.SaveChangesAsync();



            var saved = await _context.Bookings.FindAsync(3);

            Assert.NotNull(saved);

            Assert.Equal(300, saved.UserId);

        }



        [Fact]

        public async Task DeleteAsync_DeletesBooking()

        {

            await _repo.DeleteAsync(1);

            await _repo.SaveChangesAsync();



            var deleted = await _context.Bookings.FindAsync(1);

            Assert.Null(deleted);

        }



        [Fact]

        public async Task UpdateAsync_UpdatesBooking()

        {

            var booking = await _repo.GetByIdAsync(2);

            booking.Status = "Updated";

            await _repo.UpdateAsync(booking);

            await _repo.SaveChangesAsync();



            var updated = await _context.Bookings.FindAsync(2);

            Assert.Equal("Updated", updated.Status);

        }



        [Fact]

        public async Task SaveChangesAsync_ReturnsTrue_WhenChangesSaved()

        {

            var booking = new Booking { Bookingid = 4, UserId = 400 };

            await _repo.AddAsync(booking);

            var result = await _repo.SaveChangesAsync();



            Assert.True(result);

        }

    }

}