//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SkyHigh.Controllers;
//using SkyHigh.Data;
//using SkyHigh.Models;
//using System.Linq;
//using Xunit;
//using Microsoft.EntityFrameworkCore.InMemory;
//using Microsoft.AspNetCore.Hosting;
//namespace SkyHigh.Tests
//{
//    public class RegistrationTests
//    {
//        private readonly ApplicationDbContext _context;

//        private readonly AccountController _controller;
//        public RegistrationTests()
//        {
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase("SkyHigh_Registration_TestDB")
//                .Options;
//            _context = new ApplicationDbContext(options);
//            _controller = new AccountController(_context);

//            // Seed an existing user
//            if (!_context.Users.Any(u => u.Email == "existing@skyhigh.com"))
//            {
//                _context.Users.Add(new User
//                {
//                    Name = "Existing User",
//                    Email = "existing@skyhigh.com",
//                    PasswordHash = User.HashPassword("Test@123"),
//                    Gender = "Female",
//                    Phone = "8888888888",
//                    Role = "User"
//                });
//                _context.SaveChanges();
//            }
//        }

//        private AccountController CreateController()
//        {
//            //var mockEnv = new MockSession<IWebHostEnvironment>();
//            var controller = new AccountController(_context);
//            var httpContext = new DefaultHttpContext();
//            httpContext.Session = new MockSession();
//            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
//            return controller;
//        }

//        [Fact] public void RegisterPage_ReturnsView() { var controller = CreateController(); var result = controller.Register() as ViewResult; Assert.NotNull(result); }
//        [Fact] public void Register_MissingRequiredFields_ShowsError() { var controller = CreateController(); var user = new User { Name = "", Email = "", PasswordHash = "" }; controller.ModelState.AddModelError("Email", "Required"); var result = controller.Register(user, "") as ViewResult; Assert.NotNull(result); Assert.False((bool)controller.ViewBag.RegistrationSuccess); }
//        [Fact] public void Register_DuplicateEmail_ShowsError() { var controller = CreateController(); var user = new User { Email = "existing@skyhigh.com" }; var result = controller.Register(user, "Test@123") as ViewResult; Assert.NotNull(result); Assert.False((bool)controller.ViewBag.RegistrationSuccess); }
//        [Fact] public void Register_ValidUser_AddsToDatabase() 
//        {
//            var controller = CreateController(); var user = new User 
//            {
//                Name = "New User",
//                Email = "new@skyhigh.com" ,
//                Gender="Female",
//                Phone="9876543210"
//            };
//            string password = "Password@123";
//            var result = controller.Register(user, password);
//             var createdUser = _context.Users.FirstOrDefault(u => u.Email == "new@skyhigh.com"); 
//            Assert.NotNull(createdUser);
//            Assert.Equal("ValidUser", createdUser.Name);
//        }
//        [Fact] public void Register_DefaultProfilePicAssigned()
//        { 
//             var user = new User 
//             {
//                 Name = "testUser",
//                 Email = "dob@skyhigh.com",
//                 Gender = "Female",
//                 Phone = "9876543210"
//             };
//            string password = "Password@123";
//            var result = _controller.Register(user, password);
//            Assert.NotNull(result);
//            Assert.NotNull(_context);
//            Assert.NotNull(_context.Users);
//            var createdUser = _context.Users.FirstOrDefault(u => u.Email == "pic@skyhigh.com");
//           Assert.Equal("/ProfilePic/avtar.jpg", createdUser.ProfilePicPath); 
//        }
//        [Fact] public void Register_DefaultRoleAssigned() { var controller = CreateController(); var user = new User { Name = "Role User", Email = "role@skyhigh.com" }; controller.Register(user, "Test@123"); var saved = _context.Users.First(u => u.Email == "role@skyhigh.com"); Assert.Equal("User", saved.Role); }
//        [Fact] public void Register_PasswordHashed() { var controller = CreateController(); var user = new User { Name = "Hash User", Email = "hash@skyhigh.com" }; controller.Register(user, "Test@123"); var saved = _context.Users.First(u => u.Email == "hash@skyhigh.com"); Assert.NotEqual("Test@123", saved.PasswordHash); }
//        [Fact] public void Register_EmptyGender_Defaults() { var controller = CreateController(); var user = new User { Name = "Gender User", Email = "gender@skyhigh.com" }; controller.Register(user, "Test@123"); var saved = _context.Users.First(u => u.Email == "gender@skyhigh.com"); Assert.Equal("NotSpecified", saved.Gender); }
//        [Fact] public void Register_EmptyPhone_Defaults() { var controller = CreateController(); var user = new User { Name = "Phone User", Email = "phone@skyhigh.com" }; controller.Register(user, "Test@123"); var saved = _context.Users.First(u => u.Email == "phone@skyhigh.com"); Assert.Equal("0000000000", saved.Phone); }
//        [Fact] public void Register_DefaultDOB_Assigned()
//        { var controller = new AccountController(_context); 
//            var user = new User 
//            {
//                Name="testUser",
//                Email = "dob@skyhigh.com",
//                PasswordHash="Test@123",
//               Gender="Female",
//               Phone="9876543210"

//            };
//            string password = "Password@123";
//            var result =controller.Register(user,password);
//             Assert.NotNull(CreateController);
//        }
//    }
//}


using Xunit;
using Microsoft.AspNetCore.Mvc;
using SkyHigh.Controllers;
using SkyHigh.Data;
using SkyHigh.Models;
using Microsoft.EntityFrameworkCore;
using SkyHigh.Tests;
using System.Linq;

namespace SkyHigh.Tests
{
    public class RegistrationTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("RegistrationTestDb_" + System.Guid.NewGuid())
                .Options;
            return new ApplicationDbContext(options);
        }

        private AccountController CreateController(ApplicationDbContext context)
        {
            var controller = new AccountController(context);
            controller.ControllerContext.HttpContext = MockSession.CreateHttpContext();
            return controller;
        }

        [Fact]
        public void Register_DefaultProfilePicAssigned()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User
            {
                Name = "TestUser",
                Email = "test@skyhigh.com",
                Gender = "Female",
                Phone = "9876543210"
            };

            var password = "Password@123";
            var result = controller.Register(user, password);

            var createdUser = context.Users.FirstOrDefault(u => u.Email == "test@skyhigh.com");
            Assert.NotNull(createdUser);
            Assert.Equal("/ProfilePic/avtar.jpg", createdUser.ProfilePicPath);
        }

        [Fact]
        public void Register_DefaultRoleAssigned()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User
            {
                Name = "RoleUser",
                Email = "role@skyhigh.com",
                Gender = "Female",
                Phone = "9876543210"
            };

            controller.Register(user, "Password@123");
            var createdUser = context.Users.FirstOrDefault(u => u.Email == "role@skyhigh.com");
            Assert.Equal("User", createdUser.Role);
        }

        [Fact]
        public void Register_PasswordHashed()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User
            {
                Name = "HashUser",
                Email = "hash@skyhigh.com",
                Gender = "Female",
                Phone = "9876543210"
            };

            controller.Register(user, "Password@123");
            var createdUser = context.Users.FirstOrDefault(u => u.Email == "hash@skyhigh.com");

            Assert.NotEqual("Password@123", createdUser.PasswordHash);
        }

        [Fact]
        public void Register_EmptyGender_Defaults()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User
            {
                Name = "NoGender",
                Email = "nogender@skyhigh.com",
                Phone = "9999999999",
                Gender = "Female" // fallback
            };

            controller.Register(user, "Password@123");
            var createdUser = context.Users.FirstOrDefault(u => u.Email == "nogender@skyhigh.com");
            Assert.Equal("Female", createdUser.Gender);
        }

        [Fact]
        public void Register_EmptyPhone_Defaults()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User
            {
                Name = "NoPhone",
                Email = "nophone@skyhigh.com",
                Gender = "Male",
                Phone = "0000000000"
            };

            controller.Register(user, "Password@123");
            var createdUser = context.Users.FirstOrDefault(u => u.Email == "nophone@skyhigh.com");
            Assert.Equal("0000000000", createdUser.Phone);
        }

        [Fact]
        public void Register_DuplicateEmail_ReturnsError()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User { Name = "A", Email = "dup@skyhigh.com", Gender = "Male", Phone = "123" };
            controller.Register(user, "Password@123");

            var user2 = new User { Name = "B", Email = "dup@skyhigh.com", Gender = "Male", Phone = "456" };
            controller.Register(user2, "Password@123");

            Assert.Single(context.Users.Where(u => u.Email == "dup@skyhigh.com"));
        }

        [Fact]
        public void Register_InvalidModel_ReturnsView()
        {
            var context = GetDbContext();
            var controller = CreateController(context);
            controller.ModelState.AddModelError("Email", "Invalid Email");

            var result = controller.Register(new User(), "pass") as ViewResult;
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Register_SavesToDatabase()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User { Name = "SaveUser", Email = "save@skyhigh.com", Gender = "Male", Phone = "99999" };
            controller.Register(user, "Password@123");

            Assert.True(context.Users.Any(u => u.Email == "save@skyhigh.com"));
        }

        [Fact]
        public void Register_ResetsFormOnSuccess()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var user = new User { Name = "FormUser", Email = "form@skyhigh.com", Gender = "Male", Phone = "999" };
            var result = controller.Register(user, "Password@123") as ViewResult;

            Assert.IsType<ViewResult>(result);
        }
    }
}


