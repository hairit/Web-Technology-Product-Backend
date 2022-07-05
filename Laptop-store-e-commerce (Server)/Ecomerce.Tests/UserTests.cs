using Laptop_store_e_comerce.Models;
using Laptop_store_e_comerce.Repository;
using Laptop_store_e_comerce.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Ecomerce.Tests
{
    public class UserTests
    {
        private UserService userSer;
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task  getUserById_Test_True()
        {
            //Arrange
            User user = new User();
            user.Id = "user1";
            user.Firstname = "Hải";
            user.Email = "tuonghai.contact@gmail.com";
            user.Pass = "tuonghai2022";
            user.Sdt= "0777741340";
            user.Diachi = "20/1H ấp chánh 2";
            user.Mode = "ADMIN";
            user.Nameimage = "tuonghaiavatar.png";


            //var mockDBStore = new Mock<StoreContext>();
            

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(c => c.getUserByID(user.Id)).ReturnsAsync(user);
            var expectedValue = user;

            userSer = new UserService(mockUserRepo.Object);

            //Act
            var result = await userSer.getUserById(user.Id);
            var actualResult = result.Value;

            //Asserr
            Assert.That(user, Is.EqualTo(actualResult));
        }
    }
}