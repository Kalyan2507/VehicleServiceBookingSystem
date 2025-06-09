using Xunit;

using Moq;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;

using VehicleServiceBook.Controllers;

using VehicleServiceBook.Models.DTOS;

using VehicleServiceBook.Models.Domains;

using VehicleServiceBook.Repositories;

using AutoMapper;

using System.Collections.Generic;
//using NUnit.Framework;
//using VehicleServiceBook.Models.Domains;



namespace VehicleServiceBook.Tests

{

    public class AuthControllerTests

    {

        private readonly Mock<IUserRepository> _userRepositoryMock;

        private readonly Mock<IConfiguration> _configurationMock;

        private readonly Mock<IMapper> _mapperMock;

        private readonly AuthController _controller;



        public AuthControllerTests()

        {

            _userRepositoryMock = new Mock<IUserRepository>();

            _configurationMock = new Mock<IConfiguration>();

            _mapperMock = new Mock<IMapper>();



            // Provide dummy JWT config

            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForJwtGeneration123");

            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");

            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");



            _controller = new AuthController(

              _userRepositoryMock.Object,

              _configurationMock.Object,

              _mapperMock.Object

            );

        }



        [Fact]

        public async Task Login_WithValidCredentials_ReturnsJwtToken()

        {

            // Arrange

            var loginDto = new LoginDto

            {

                Email = "test@example.com",

                PasswordHash = BCrypt.Net.BCrypt.HashPassword("test123")

            };



            var user = new Registration

            {

                Email = loginDto.Email,

                PasswordHash = loginDto.PasswordHash,

                Role = "User"

            };



            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(user);



            // Act

            var result = await _controller.Login(loginDto);



            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result);

            var tokenObject = okResult.Value as dynamic;



            Assert.NotNull(tokenObject.token);

            Assert.IsType<string>(tokenObject.token);

        }



        [Fact]

        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()

        {

            // Arrange

            var loginDto = new LoginDto

            {

                Email = "test@example.com",

                PasswordHash = "wrongpassword"

            };



            var user = new Registration

            {

                Email = loginDto.Email,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),

                Role = "User"

            };



            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(user);



            // Act

            var result = await _controller.Login(loginDto);



            // Assert

            Assert.IsType<UnauthorizedObjectResult>(result);

        }



        [Fact]

        public async Task Login_WithInvalidEmail_ReturnsUnauthorized()

        {

            // Arrange

            var loginDto = new LoginDto

            {

                Email = "nonexistent@example.com",

                PasswordHash = "any"

            };



            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync((Registration)null);



            // Act

            var result = await _controller.Login(loginDto);



            // Assert

            Assert.IsType<UnauthorizedObjectResult>(result);

        }

    }

}