using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services;
using AuthApi.Services.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthApi.UnitTests
{
    public class AuthServiceRegisterTests
    {
        private readonly AppDbContext _dbContext;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IJwtTokenGenerator> _jwtMock;

        private readonly AuthService serviceUnderTest;

        public AuthServiceRegisterTests()
        {
            _dbContext = GetDbContext();

            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();
            _jwtMock = new Mock<IJwtTokenGenerator>();

            serviceUnderTest = new AuthService(
                _dbContext,
                _jwtMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object
            );
        }

        // SUCCESS
        [Fact]
        public async Task Register_ShouldSucceed_WhenUserIsValid()
        {
            var request = new RegistrationRequestDto
            { 
                Email = "new@test.com",
                Password = "Password123!",
                Name = "Test",
                PhoneNumber = "123456"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);

            var result = await serviceUnderTest.Register(request);

            Assert.True(result == null || result.Count == 0);

            _userManagerMock.Verify(x =>
                x.CreateAsync(It.Is<ApplicationUser>(u => u.Email == request.Email), request.Password),
                Times.Once);
        }

        // EMAIL EXISTS
        [Fact]
        public async Task Register_ShouldFail_WhenEmailAlreadyExists()
        {
            var existingUser = new ApplicationUser
            {
                UserName = "test@test.com",
                Email = "test@test.com"
            };

            _dbContext.ApplicationUsers.Add(existingUser);
            _dbContext.SaveChanges();

            var request = new RegistrationRequestDto
            {
                Email = "test@test.com",
                Password = "Password123!"
            };

            var result = await serviceUnderTest.Register(request);

            Assert.NotNull(result);
            Assert.Contains(result, e => e.Field == "Email");

            _userManagerMock.Verify(x =>
                x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
                Times.Never);
        }

        // IDENTITY FAILURE (e.g. weak password)
        [Fact]
        public async Task Register_ShouldReturnErrors_WhenIdentityFails()
        {
            var request = new RegistrationRequestDto
            {
                Email = "new@test.com",
                Password = "weak"
            };

            var identityErrors = new List<IdentityError>
            {
                new() { Code = "PasswordTooShort", Description = "Password too short" }
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed([.. identityErrors]));

            var result = await serviceUnderTest.Register(request);

            Assert.NotNull(result);
            Assert.Contains(result, e => e.Field == "Password");
        }

        // EXCEPTION HANDLING
        [Fact]
        public async Task Register_ShouldReturnGenericError_WhenExceptionOccurs()
        {
            var request = new RegistrationRequestDto
            {
                Email = "new@test.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ThrowsAsync(new Exception("DB failure"));

            var result = await serviceUnderTest.Register(request);

            Assert.NotNull(result);
            Assert.Contains(result, e => e.Message == "Error Encountered");
        }

        // ---------------- HELPERS ----------------

        private static AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        private static Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();

            return new Mock<RoleManager<IdentityRole>>(
                store.Object,
                null, null, null, null
            );
        }
    }
}