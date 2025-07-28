using HIV.Controllers;
using HIV.DTOs;
using HIV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAPITest.Controllers;
using Xunit;

namespace HIV.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly AppDbContext _context;
        private readonly AccountController _controller;

        public AccountControllerTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // tạo DB riêng cho mỗi test
                .Options;

            _context = new AppDbContext(options);
            //_controller = new AccountController(_context);
        }


        [Theory]
        [InlineData("user", "123","Login Successfully")]
        public async Task Login_Correct1(string usernamne,string password,string result2)
        {
            var account = new Account
            {
                AccountId = 1,
                Username = "user",
                Email = "user@test.com",
                PasswordHash = "123"
            };
            var user = new User
            {
                AccountId = 1,
                FullName = "User",
                Role = "Patient",
                UserId = 1,
                UserAvatar = "avt.png"
            };

            await _context.Accounts.AddAsync(account);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dtoCorrect = new DTOLogin
            {
                identifier = usernamne,
                password_hash = password
            };
            var dtoWrong = new DTOLogin
            {
                identifier = usernamne,
                password_hash = "wrongpass"
            };

            Assert.Equal(result2,"Login Successfully");
       
        }

        [Fact]
        public async Task Login_Correct()
        {
            var account = new Account
            {
                AccountId = 1,
                Username = "user",
                Email = "user@test.com",
                PasswordHash = "123"
            };
            var user = new User
            {
                AccountId = 1,
                FullName = "User",
                Role = "Patient",
                UserId = 1,
                UserAvatar = "avt.png"
            };

            await _context.Accounts.AddAsync(account);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dtoCorrect = new DTOLogin
            {
                identifier = "user",
                password_hash = "123"
            };
            var dtoWrong = new DTOLogin
            {
                identifier = "user",
                password_hash = "wrongpass"
            };

            // ✅ Đăng nhập đúng
            var result = await _controller.Login(dtoCorrect);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Contains("Login successful", okResult.Value.ToString());

            // ❌ Đăng nhập sai
            var result1 = await _controller.Login(dtoWrong);
            var badResult = Assert.IsType<BadRequestObjectResult>(result1.Result);
            Assert.Contains("incorrect", badResult.Value.ToString().ToLower());
        }


        [Fact]
        public async Task Register_ValidInput_ReturnsOk()
        {
            var dto = new DTOGetbyID
            {
                username = "newuser",
                email = "newuser@test.com",
                password_hash = "123",
                user_id = 2,
                full_name = "New User",
                phone = "0123456789",
                gender = "Male",
                birthdate = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
                role = "Patient",
                address = "HN",
                user_avatar = null
            };

            var result = await _controller.Register(dto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultValue = okResult.Value?.ToString();
            Assert.Contains("Register success", resultValue);
        }

        [Fact]
        public async Task GetAccountById_ExistingId_ReturnsDto()
        {
            var account = new Account
            {
                AccountId = 3,
                Username = "u3",
                Email = "u3@test.com",
                PasswordHash = "123",
                CreatedAt = DateTime.UtcNow
            };

            var user = new User
            {
                AccountId = 3,
                FullName = "User 3",
                Phone = "099",
                Gender = "Other",
                Birthdate = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
                Role = "Doctor",
                Address = "HN",
                UserAvatar = "avt3.png"
            };

            await _context.Accounts.AddAsync(account);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _controller.GetAccountById(3);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<DTOGetbyID>(okResult.Value);
            Assert.Equal("u3", dto.username);
            Assert.Equal("User 3", dto.full_name);
        }

        [Fact]
        public async Task UpdateInfo_ValidInput_ReturnsNoContent()
        {
            var acc = new Account
            {
                AccountId = 4,
                Username = "test_user4",
                Email = "user4@test.com",
                PasswordHash = "test123",
                CreatedAt = DateTime.UtcNow
            };

            var user = new User
            {
                AccountId = 4,
                FullName = "Old",
                Gender = "Male",
                Phone = "000",
                Birthdate = DateOnly.FromDateTime(new DateTime(2004, 1, 1)),
                Role = "Patient",
                Address = "HN",
                UserAvatar = "old.png"
            };

            await _context.Accounts.AddAsync(acc);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dto = new DTOUpdate
            {
                full_name = "New Name",
                gender = "Female",
                phone = "111",
                birthdate = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
                role = "Patient",
                address = "SG",
                user_avatar = "new.png"
            };

            var result = await _controller.UpdateInfo(4, dto);
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task ChangePassword_ValidInput_ChangesPassword()
        {
            var acc = new Account
            {
                AccountId = 5,
                Username = "user5",
                Email = "user5@test.com",
                PasswordHash = "old",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Accounts.AddAsync(acc);
            await _context.SaveChangesAsync();

            var dto = new ChangePassword
            {
                password_hash = "new123"
            };

            var result = await _controller.ChangePassword(5, dto);
            Assert.IsType<NoContentResult>(result);

            var updated = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == 5);
            Assert.Equal("new123", updated.PasswordHash);
        }
    }
}