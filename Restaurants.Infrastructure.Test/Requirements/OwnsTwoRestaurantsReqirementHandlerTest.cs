using Microsoft.AspNetCore.Authorization;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Authorization.Requirements.OwnsTwoRestaurants;

namespace Restaurants.Infrastructure.Test.Requirements
{


    public class OwnsTwoRestaurantsReqirementHandlerTest
    {
        private readonly Mock<IRestaurantsRepository> restaurantRepositoryMock;
        private readonly Mock<IUserContext> userContextMock;
        public OwnsTwoRestaurantsReqirementHandlerTest()
        {
            restaurantRepositoryMock = new Mock<IRestaurantsRepository>();
            userContextMock = new Mock<IUserContext>();
        }
        [Fact]
        public async Task HandleRequirementAsync_UserHasNotCreateMoreThanTwoRestaurants_ShouldFail()
        {

            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>()
            {
                new(){OwnerId = currentUser.Id},
                new (){OwnerId = "2"},
            };

            restaurantRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var requirements = new OwnsTwoRestaurantsReqirement(2);
            var handler = new OwnsTwoRestaurantsReqirementHandler(restaurantRepositoryMock.Object, userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirements], null, null);

            // Act

            await handler.HandleAsync(context);

            // Assert

            Assert.False(context.HasSucceeded);

            restaurantRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);

            userContextMock.Verify(u => u.GetCurrentUser(), Times.Once);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasCreatedMoreThanTwoRestaurants_ShouldSucceded()
        {

            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);

            userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>()
            {
                new(){OwnerId = currentUser.Id},
                new(){OwnerId = currentUser.Id},
                new (){OwnerId = "2"},
            };

            restaurantRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var requirements = new OwnsTwoRestaurantsReqirement(2);
            var handler = new OwnsTwoRestaurantsReqirementHandler(restaurantRepositoryMock.Object, userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirements], null, null);


            await handler.HandleAsync(context);

            Assert.True(context.HasSucceeded);

            restaurantRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);

            userContextMock.Verify(u => u.GetCurrentUser(), Times.Once);
        }
    }
}
