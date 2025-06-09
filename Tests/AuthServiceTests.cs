using Xunit;

using Moq;

using Microsoft.Extensions.Configuration;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;

using System.Threading.Tasks;

using System.Linq;

using VehicleServiceBook.Services;

using VehicleServiceBook.Models.Domains;

using System;

using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Text;



namespace VehicleServiceBook.Test 
{

    public class AuthServiceTests

    {

        private readonly Mock<IConfiguration> _configMock;

        private readonly VehicleServiceBookContext _context;

        private readonly AuthService _authService;



        public AuthServiceTests()

        {

            // Setup fake configuration

            _configMock = new Mock<IConfiguration>();

            _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperSecretJwtKey1234567890");

            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");

            _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");



            // Setup fake in-memory DB context

            var options = new DbContextOptionsBuilder<VehicleServiceBookContext>()

        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())

        .Options;



            _context = new VehicleServiceBookContext(options);



            // Seed a test user

            _context.Registrations.Add(new Registration

            {

                Email = "test@example.com",

                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),

                Role = "User"

            });

            _context.SaveChanges();



            // Create service instance

            _authService = new AuthService(_context, _configMock.Object);

        }



        [Fact]

        public async Task AuthenticateAsync_WithValidCredentials_ReturnsToken()

        {

            // Act

            var token = await _authService.AuthenticateAsync("test@example.com", "password123");



            // Assert

            Assert.NotNull(token);

            Assert.IsType<string>(token);

        }



        [Fact]

        public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()

        {

            var token = await _authService.AuthenticateAsync("test@example.com", "wrongpassword");

            Assert.Null(token);

        }



        [Fact]

        public async Task AuthenticateAsync_WithInvalidEmail_ReturnsNull()

        {

            var token = await _authService.AuthenticateAsync("notfound@example.com", "password123");

            Assert.Null(token);

        }

    }

}