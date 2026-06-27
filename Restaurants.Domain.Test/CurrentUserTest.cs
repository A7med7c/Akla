using Microsoft.AspNetCore.Http;
using Moq;

using Restaurants.Domain.Constants;
using System.Security.Claims;
namespace Restaurants.Domain.Test
{
    public class CurrentUserTest
    {
        [Fact]
        public void GetCurrentUser_WithAuthenticatedUser_SholudReturnCurrentUser()
        {

            //Arrange 
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var userClaims = new List<Claim>()
            {
                new (ClaimTypes.NameIdentifier ,"1"),
                new (ClaimTypes.Email ,"test@test.com"),
                new (ClaimTypes.Role ,UserRoles.Admin),
                new (ClaimTypes.Role ,UserRoles.User),
                new ("Nationality" ,"Egyptian"),
                new ("dateOfBirth" ,"2002-02-02"),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "test"));

            httpContextAccessorMock.Setup(x => x.HttpContext)
                               .Returns(new DefaultHttpContext
                               {
                                   User = user
                               });

            var userContext = new Application.Users.UserContext(httpContextAccessorMock.Object);

            //act 

            var currentUser = userContext.GetCurrentUser();
            //assert

            Assert.NotNull(currentUser);
            Assert.Equal("1", currentUser.Id);
            Assert.Equal("test@test.com", currentUser.Email);
            Assert.Contains(UserRoles.Admin, currentUser.Roles.First());
        }
    }
}

