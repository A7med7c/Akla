using Microsoft.AspNetCore.Authorization;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Authorization.Requirements.OwnsTwoRestaurants;

namespace Restaurants.Infrastructure.Test.Requirements
{
    public class OwnsTwoRestaurantsReqirementHandlerTest
    {
        [Fact]
        public async Task HandleRequirementAsync_UserHasNotCreateMoreThanTwoRestaurants_ShouldFail()
        {
            var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);

            var userContexrMock = new Mock<IUserContext>();

            userContexrMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>()
            {
                new(){OwnerId = currentUser.Id},
                new (){OwnerId = "2"},
            };

            restaurantRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var requirements = new OwnsTwoRestaurantsReqirement(2);
            var handler = new OwnsTwoRestaurantsReqirementHandler(restaurantRepositoryMock.Object, userContexrMock.Object);
            var context = new AuthorizationHandlerContext([requirements], null, null);

            //act 
            await handler.HandleAsync(context);

            context.Fail();
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasCreatedMoreThanTwoRestaurants_ShoulSucceded()
        {
            var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);

            var userContexrMock = new Mock<IUserContext>();

            userContexrMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>()
            {
                new(){OwnerId = currentUser.Id},
                new(){OwnerId = currentUser.Id},
                new (){OwnerId = "2"},
            };

            restaurantRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var requirements = new OwnsTwoRestaurantsReqirement(2);
            var handler = new OwnsTwoRestaurantsReqirementHandler(restaurantRepositoryMock.Object, userContexrMock.Object);
            var context = new AuthorizationHandlerContext([requirements], null, null);


            context.Succeed(requirements);
        }
    }
}
