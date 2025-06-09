using Xunit;

using Moq;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using System.Security.Claims;

using VehicleServiceBook.Controllers;

using VehicleServiceBook.Models.Domains;

using VehicleServiceBook.Models.DTOS;

using VehicleServiceBook.Repositories;

using AutoMapper;

using Microsoft.AspNetCore.Http;

using VehicleServiceBook.Models;



namespace VehicleServiceBook.Tests 

{

    public class BookingControllerTests

    {

        private readonly Mock<IBookingRepository> _bookingRepoMock;

        private readonly Mock<IUserRepository> _userRepoMock;

        private readonly Mock<IMapper> _mapperMock;

        private readonly BookingController _controller;



        public BookingControllerTests()

        {

            _bookingRepoMock = new Mock<IBookingRepository>();

            _userRepoMock = new Mock<IUserRepository>();

            _mapperMock = new Mock<IMapper>();



            var dbContextMock = new Mock<VehicleServiceBookContext>(); // not used directly

            _controller = new BookingController(dbContextMock.Object, _bookingRepoMock.Object, _userRepoMock.Object, _mapperMock.Object);



            // Set up mock user in context

            var userClaims = new List<Claim>

      {

        new Claim(ClaimTypes.Email, "test@example.com"),

        new Claim(ClaimTypes.Role, "User")

      };



            var identity = new ClaimsIdentity(userClaims, "TestAuthType");

            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext()

            {

                HttpContext = new DefaultHttpContext { User = principal }

            };

        }



        [Fact]

        public async Task GetAll_ReturnsMappedBookings()

        {

            // Arrange

            var user = new Registration { UserId = 1, Email = "test@example.com" };

            _userRepoMock.Setup(x => x.GetUserByEmailAsync("test@example.com")).ReturnsAsync(user);



            var bookings = new List<Booking> { new Booking { Bookingid = 1, UserId = 1 } };

            _bookingRepoMock.Setup(x => x.GetAllByUserIdAsync(1)).ReturnsAsync(bookings);



            var bookingDtos = new List<BookingDto> { new BookingDto { BookingId = 1 } };

            _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);



            // Act

            var result = await _controller.GetAll();



            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnValue = Assert.IsAssignableFrom<IEnumerable<BookingDto>>(okResult.Value);

            Assert.Single(returnValue);

        }



        [Fact]

        public async Task GetById_BookingFound_ReturnsBookingDto()

        {

            // Arrange

            var booking = new Booking { Bookingid = 2 };

            _bookingRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(booking);

            _mapperMock.Setup(m => m.Map<BookingDto>(booking)).Returns(new BookingDto { BookingId = 2 });



            // Act

            var result = await _controller.GetById(2);



            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result);

            var bookingDto = Assert.IsType<BookingDto>(okResult.Value);

            Assert.Equal(2, bookingDto.BookingId);

        }



        [Fact]

        public async Task GetById_BookingNotFound_ReturnsNotFound()

        {

            _bookingRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Booking)null);



            var result = await _controller.GetById(10);



            Assert.IsType<NotFoundResult>(result);

        }



        [Fact]

        public async Task Delete_ValidId_ReturnsNoContent()

        {

            // Arrange

            var booking = new Booking { Bookingid = 1 };

            _bookingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);

            _bookingRepoMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            _bookingRepoMock.Setup(r => r.SaveChangesAsync()).Returns((Task<bool>)Task.CompletedTask);



            // Act

            var result = await _controller.Delete(1);



            // Assert

            Assert.IsType<NoContentResult>(result);

        }



        [Fact]

        public async Task Delete_InvalidId_ReturnsNotFound()

        {

            _bookingRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Booking)null);



            var result = await _controller.Delete(999);



            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("No booking found with ID 999", notFound.Value);

        }

    }

}