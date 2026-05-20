using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services;
using AuthApi.Services.Interfaces;
using Contracts.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthApi.UnitTests
{
    public class AuthServiceLoginTests
    {
        AuthService serviceUnderTest;

        private readonly AppDbContext _dbContext;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IJwtTokenGenerator> _jwtMock;

        private readonly ApplicationUser user = new ApplicationUser
        {
            Id = "1",
            UserName = "test@test.com",
            Email = "test@test.com",
            Name = "Test User"
        };

        public AuthServiceLoginTests()
        {
            _dbContext = GetDbContext(user);

            _jwtMock = new Mock<IJwtTokenGenerator>();
            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();

            serviceUnderTest = new AuthService(
                 _dbContext,
                 _jwtMock.Object,
                 _userManagerMock.Object,
                 _roleManagerMock.Object
             );
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            SetupValidLogin();

            var request = new LoginRequestDto
            {
                UserName = "test@test.com",
                Password = "password"
            };

            // Act
            var result = await serviceUnderTest.Login(request);

            // Assert
            result.Should().NotBeNull();
            result.User.Should().NotBeNull();
            result.Token.Should().Be("fake-jwt");

            _jwtMock.Verify(x => x.GenerateToken(user,
                It.Is<IEnumerable<string>>(roles => roles.Contains("Admin"))), Times.Once);

            _userManagerMock.Verify(x => x.CheckPasswordAsync(user, "password"), Times.Once);
        }


        [Fact]
        public async Task Login_ShouldNotReturnToken_WhenEmailIsInvalid()
        {
            SetupValidLogin();

            var request = new LoginRequestDto
            {
                UserName = "wrong@test.com",
                Password = "password"
            };

            // Act
            var result = await serviceUnderTest.Login(request);

            // Assert
            result.Should().NotBeNull();
            result.User.Should().BeNull();
            result.Token.Should().Be("");
            result.Error.Field.Should().Be("UserName");
            result.Error.Message.Should().Be("User does not exist");

            _jwtMock.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);

        }

        [Fact]
        public async Task Login_ShouldNotReturnToken_WhenPasswordIsInvalid()
        {
            // no setup needed — user won't be found
            //_userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password"))
            //    .ReturnsAsync(true);

            //_userManagerMock.Setup(x => x.GetRolesAsync(user))
            //    .ReturnsAsync(new List<string> { "Admin" });

            //_jwtMock.Setup(x => x.GenerateToken(user, It.IsAny<IEnumerable<string>>()))
            //    .Returns("fake-jwt");

            SetupValidLogin();

            var request = new LoginRequestDto
            {
                UserName = "test@test.com",
                Password = "DifferentPassword"
            };

            // Act
            var result = await serviceUnderTest.Login(request);

            // Assert
            result.Should().NotBeNull();
            result.User.Should().BeNull();
            result.Token.Should().Be("");
            result.Error.Field.Should().Be("Password");
            result.Error.Message.Should().Be("Invalid password");

            _jwtMock.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }
        [Fact]
        public async Task Login_ShouldBeCaseInsensitive_ForUsername()
        {
            var request = new LoginRequestDto
            {
                UserName = "TEST@TEST.COM",
                Password = "password"
            };

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password"))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            _jwtMock.Setup(x => x.GenerateToken(user, It.IsAny<IEnumerable<string>>()))
                .Returns("fake-jwt");

            var result = await serviceUnderTest.Login(request);

            result.User.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_ShouldFail_WhenPasswordIsEmpty()
        {
            var request = new LoginRequestDto
            {
                UserName = "test@test.com",
                Password = ""
            };

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, ""))
                .ReturnsAsync(false);

            var result = await serviceUnderTest.Login(request);

            result.User.Should().BeNull();
            result.Token.Should().Be("");
            result.Error.Field.Should().Be("Password");
            result.Error.Message.Should().Be("Password is required");
        }

        [Fact]
        public async Task Login_ShouldFail_WhenPasswordIsNull()
        {
            var request = new LoginRequestDto
            {
                UserName = "test@test.com",
                Password = null
            };

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, ""))
                .ReturnsAsync(false);

            var result = await serviceUnderTest.Login(request);

            result.User.Should().BeNull();
            result.Token.Should().Be("");
            result.Error.Field.Should().Be("Password");
            result.Error.Message.Should().Be("Password is required");
        }

        [Fact]
        public async Task Login_ShouldFail_WhenUserNameIsNull()
        {
            var request = new LoginRequestDto
            {
                UserName = null,
                Password = "password"
            };

            var result = await serviceUnderTest.Login(request);

            result.User.Should().BeNull();
        }


        // ---------------- HELPERS ----------------
        private AppDbContext GetDbContext(ApplicationUser user)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.ApplicationUsers.Add(user);
            context.SaveChanges();

            return context;
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        private Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();

            return new Mock<RoleManager<IdentityRole>>(
                store.Object,
                null, null, null, null
            );
        }

        private void SetupValidLogin()
        {
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password"))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            _jwtMock.Setup(x => x.GenerateToken(user, It.IsAny<IEnumerable<string>>()))
                .Returns("fake-jwt");
        }
    }
}