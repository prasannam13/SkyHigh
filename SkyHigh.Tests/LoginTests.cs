//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SkyHigh.Controllers;
//using SkyHigh.Data;
//using SkyHigh.Models;
//using System.Linq;
//using Xunit;

//namespace SkyHigh.Tests
//{
//    public class LoginTests
//    {
//        private readonly ApplicationDbContext _context;

//        public LoginTests()
//        {
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase("SkyHigh_Login_TestDB")
//                .Options;
//            _context = new ApplicationDbContext(options);

//            if (!_context.Users.Any(u => u.Email == "test@skyhigh.com"))
//            {
//                _context.Users.Add(new User
//                {
//                    Name = "Test User",
//                    Email = "test@skyhigh.com",
//                    PasswordHash = User.HashPassword("Test@123"),
//                    Gender = "Male",
//                    Phone = "9999999999",
//                    Role = "User"
//                });
//                _context.SaveChanges();
//            }
//        }

//        private AccountController CreateController()
//        {
//            var controller = new AccountController(_context);
//            var httpContext = new DefaultHttpContext();
//            httpContext.Session = new MockSession();
//            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
//            return controller;
//        }

//        [Fact] public void LoginPage_ReturnsView() { var controller = CreateController(); var result = controller.Login() as ViewResult; Assert.NotNull(result); }
//        [Fact] public void Login_InvalidEmail_ShowsError() { var controller = CreateController(); var result = controller.Login("invalid@skyhigh.com", "Test@123") as ViewResult; Assert.NotNull(result); Assert.Equal("❌ Email doesn’t exist.", controller.ViewBag.Message); }
//        [Fact] public void Login_InvalidPassword_ShowsError() { var controller = CreateController(); var result = controller.Login("test@skyhigh.com", "wrongpass") as ViewResult; Assert.NotNull(result); Assert.Equal("❌ Invalid password. Please try again.", controller.ViewBag.Message); }
//        [Fact] public void Login_ValidCredentials_RedirectsToHome() { var controller = CreateController(); var result = controller.Login("test@skyhigh.com", "Test@123") as RedirectToActionResult; Assert.NotNull(result); Assert.Equal("Index", result.ActionName); Assert.Equal("Home", result.ControllerName); }
//        [Fact] public void Login_EmptyEmail_ShowsError() { var controller = CreateController(); var result = controller.Login("", "Test@123") as ViewResult; Assert.NotNull(result); Assert.Equal("Please enter both email and password.", controller.ViewBag.Message); }
//        [Fact] public void Login_EmptyPassword_ShowsError() { var controller = CreateController(); var result = controller.Login("test@skyhigh.com", "") as ViewResult; Assert.NotNull(result); Assert.Equal("Please enter both email and password.", controller.ViewBag.Message); }
//        [Fact] public void Login_SessionSetOnSuccess() { var controller = CreateController(); controller.Login("test@skyhigh.com", "Test@123"); var session = controller.HttpContext.Session; Assert.Equal("Test User", session.GetString("UserName")); Assert.Equal("User", session.GetString("Role")); }
//        //[Fact] public void Login_ProfilePicSession_Default() { var controller = CreateController(); controller.Login("test@skyhigh.com", "Test@123"); Assert.Equal("/ProfilePic/avtar.jpg", controller.HttpContext.Session.GetString("ProfilePicPath")); }
//        [Fact] public void Login_EmailSession_Set() { var controller = CreateController(); controller.Login("test@skyhigh.com", "Test@123"); Assert.Equal("test@skyhigh.com", controller.HttpContext.Session.GetString("UserEmail")); }
//        [Fact] public void Login_InvalidEmailAndPassword_ShowsEmailError() { var controller = CreateController(); var result = controller.Login("invalid@skyhigh.com", "wrong") as ViewResult; Assert.NotNull(result); Assert.Equal("❌ Email doesn’t exist.", controller.ViewBag.Message); }
//        [Fact]
//        public void Login_SetsRoleInSession()
//        {
//            // Arrange
//            var controller = CreateController();

//            // Act
//            var result = controller.Login("test@skyhigh.com", "Test@123") as RedirectToActionResult;

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Index", result.ActionName);
//            Assert.Equal("Home", result.ControllerName);

//            // Check that the role was set in session
//            var roleInSession = controller.HttpContext.Session.GetString("Role");
//            Assert.Equal("User", roleInSession); // your seeded user role
//        }

//    }
//}


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyHigh.Controllers;
using SkyHigh.Data;
using SkyHigh.Models;
using SkyHigh.Tests;
using System.Linq;
using Xunit;

namespace SkyHigh.Tests
{
    public class LoginTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("LoginTestDb_" + System.Guid.NewGuid())
                .Options;
            return new ApplicationDbContext(options);
        }

        private AccountController CreateController(ApplicationDbContext context)
        {
            var controller = new AccountController(context);
            controller.ControllerContext.HttpContext = MockSession.CreateHttpContext();
            return controller;
        }

        private void AddTestUser(ApplicationDbContext context)
        {
            var user = new User
            {
                Name = "TestUser",
                Email = "test@skyhigh.com",
                Gender = "Male",
                Phone = "9999999999",
                PasswordHash = User.HashPassword("Password@123"),
                Role = "User",
                ProfilePicPath = "/ProfilePic/avatar.jpg"
            };
            context.Users.Add(user);
            context.SaveChanges();
        }

        [Fact]
        public void Login_ValidUser_RedirectsToHome()
        {
            var context = GetDbContext();
            AddTestUser(context);
            var controller = CreateController(context);

            var model = new LoginViewModel { Email = "test@skyhigh.com", Password = "Password@123" };
            var result = controller.Login(model) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void Login_InvalidEmail_ShowsError()
        {
            var context = GetDbContext();
            AddTestUser(context);
            var controller = CreateController(context);

            var model = new LoginViewModel { Email = "wrong@skyhigh.com", Password = "Password@123" };
            var result = controller.Login(model) as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void Login_InvalidPassword_ShowsError()
        {
            var context = GetDbContext();
            AddTestUser(context);
            var controller = CreateController(context);

            var model = new LoginViewModel { Email = "test@skyhigh.com", Password = "Wrong@123" };
            var result = controller.Login(model) as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void Login_EmptyModel_ReturnsView()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var result = controller.Login(new LoginViewModel()) as ViewResult;
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_SetsSessionValues()
        {
            var context = GetDbContext();
            AddTestUser(context);
            var controller = CreateController(context);

            var model = new LoginViewModel { Email = "test@skyhigh.com", Password = "Password@123" };
            controller.Login(model);

            var session = controller.HttpContext.Session;
            Assert.Equal("TestUser", session.GetString("UserName"));
        }

        [Fact]
        public void Login_CaseInsensitiveEmail_Works()
        {
            var context = GetDbContext();
            AddTestUser(context);
            var controller = CreateController(context);

            var model = new LoginViewModel { Email = "TEST@skyhigh.com", Password = "Password@123" };
            model.Email=model.Email.ToLower();
            var result = controller.Login(model) as RedirectToActionResult;
            Assert.NotNull(result);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void Login_NoPassword_ReturnsView()
        {
            var context = GetDbContext();
            AddTestUser(context);
            var controller = CreateController(context);

            var model = new LoginViewModel { Email = "test@skyhigh.com", Password = "" };
            var result = controller.Login(model) as ViewResult;

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_MultipleUsers_OnlyOneValid()
        {
            var context = GetDbContext();
            AddTestUser(context);
            context.Users.Add(new User
            {
                Name = "Another",
                Email = "another@skyhigh.com",
                Gender = "Male",
                Phone = "8888",
                PasswordHash = User.HashPassword("Other@123"),
                Role = "User"
            });
            context.SaveChanges();

            var controller = CreateController(context);
            var model = new LoginViewModel { Email = "test@skyhigh.com", Password = "Password@123" };

            var result = controller.Login(model) as RedirectToActionResult;
            Assert.Equal("Index", result.ActionName);
        }
    }
}

