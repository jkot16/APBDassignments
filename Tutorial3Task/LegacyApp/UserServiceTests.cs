using System;
using Xunit;

namespace LegacyApp.Tests
{
    public class UserServiceTests
    {
        
        [Fact]
        public void AddUser_ReturnsFalse_WhenEmailIsInvalid()
        {
            var service = new UserService(new ClientRepository(), new UserCreditService());
            var result = service.AddUser("Jacob", "Doe", "kociakexample.com", DateTime.Now.AddYears(-30), 1);
            Assert.False(result);
        }
        
        [Fact]
        public void AddUser_ReturnsFalse_WhenTooYoung()
        {
            var service = new UserService(new ClientRepository(), new UserCreditService());
            var result = service.AddUser("Jacob", "Doe", "kociak@example.com", DateTime.Now.AddYears(-20), 1);
            Assert.False(result);
        }
        
        [Fact]
        public void AddUser_ReturnsTrue_ForValidUser_NormalClient()
        {
            var service = new UserService(new ClientRepository(), new UserCreditService());
            var result = service.AddUser("Jacob", "Doe", "kociak@example.com", DateTime.Now.AddYears(-30), 1);
            Assert.True(result);
        }
    }
}