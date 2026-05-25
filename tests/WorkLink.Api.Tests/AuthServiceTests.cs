using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using WorkLink.Api.DTOs;
using WorkLink.Api.Models;
using WorkLink.Api.Services;

namespace WorkLink.Api.Tests;

public class AuthServiceTests
{
    private static List<ApplicationUser> _users = [];

    private static UserManager<ApplicationUser> CreateUserManager()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        var mgr = Substitute.For<UserManager<ApplicationUser>>(store, null, null, null, null, null, null, null, null);

        mgr.FindByEmailAsync(Arg.Any<string>()).Returns(call =>
            _users.FirstOrDefault(u => u.Email == call.Arg<string>()));

        mgr.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(call =>
        {
            _users.Add(call.Arg<ApplicationUser>());
            return IdentityResult.Success;
        });

        mgr.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        mgr.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(call =>
            _users.Contains(call.Arg<ApplicationUser>()) ? ["Client"] : []);

        mgr.CheckPasswordAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(call =>
        {
            var user = call.Arg<ApplicationUser>();
            return _users.Contains(user) && call.Arg<string>() == "CorrectPass1!";
        });

        return mgr;
    }

    private static RoleManager<IdentityRole> CreateRoleManager()
    {
        var roleStore = Substitute.For<IRoleStore<IdentityRole>>();
        var rm = Substitute.For<RoleManager<IdentityRole>>(roleStore, null, null, null, null);
        rm.RoleExistsAsync(Arg.Any<string>()).Returns(true);
        rm.CreateAsync(Arg.Any<IdentityRole>()).Returns(IdentityResult.Success);
        return rm;
    }

    private static IConfiguration CreateConfig()
    {
        var config = Substitute.For<IConfiguration>();
        config["Jwt:Key"].Returns("3d247f96586c1432b272f8a16181f32f6a02e2c9d6008a72c942140a54f3c4d2");
        config["Jwt:Issuer"].Returns("WorkLink.Api");
        config["Jwt:Audience"].Returns("WorkLink.Client");
        return config;
    }

    [Before(Test)]
    public void Cleanup() => _users = [];

    [Test]
    public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperation()
    {
        _users.Add(new ApplicationUser { Email = "dup@test.com" });
        var service = new AuthService(CreateUserManager(), CreateRoleManager(), CreateConfig());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(new RegisterRequest
            {
                Email = "dup@test.com", Password = "Test123!",
                DisplayName = "Dup", Role = "Client"
            }));

        await Assert.That(ex?.Message).IsEqualTo("Email already registered.");
    }

    [Test]
    public async Task RegisterAsync_ValidRequest_ReturnsAuthResponse()
    {
        var service = new AuthService(CreateUserManager(), CreateRoleManager(), CreateConfig());

        var result = await service.RegisterAsync(new RegisterRequest
        {
            Email = "new@test.com", Password = "Test123!",
            DisplayName = "New User", Role = "Client"
        });

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Email).IsEqualTo("new@test.com");
        await Assert.That(result.DisplayName).IsEqualTo("New User");
        await Assert.That(result.Role).IsEqualTo("Client");
        await Assert.That(result.Token).IsNotEmpty();
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorized()
    {
        _users.Add(new ApplicationUser { Id = "1", Email = "user@test.com", DisplayName = "User", UserName = "user@test.com" });
        var service = new AuthService(CreateUserManager(), CreateRoleManager(), CreateConfig());

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginRequest { Email = "user@test.com", Password = "WrongPass1!" }));

        await Assert.That(ex?.Message).IsEqualTo("Invalid email or password.");
    }

    [Test]
    public async Task LoginAsync_NonexistentEmail_ThrowsUnauthorized()
    {
        var service = new AuthService(CreateUserManager(), CreateRoleManager(), CreateConfig());

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginRequest { Email = "nobody@test.com", Password = "Test123!" }));

        await Assert.That(ex?.Message).IsEqualTo("Invalid email or password.");
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        _users.Add(new ApplicationUser { Id = "1", Email = "valid@test.com", DisplayName = "Valid", UserName = "valid@test.com" });
        var service = new AuthService(CreateUserManager(), CreateRoleManager(), CreateConfig());

        var result = await service.LoginAsync(new LoginRequest { Email = "valid@test.com", Password = "CorrectPass1!" });

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Email).IsEqualTo("valid@test.com");
        await Assert.That(result.Token).IsNotEmpty();
    }
}
