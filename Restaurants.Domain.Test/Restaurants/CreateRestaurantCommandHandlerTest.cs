using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;

namespace Restaurants.Domain.Test.Restaurants
{
    public class CreateRestaurantCommandHandlerTest
    {
        [Fact]
        public async Task Handle_ForValidCommnd_ReturnsCreatedRestaurantId()
        {

            //arrange 
            var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
            var mapperMock = new Mock<IMapper>();

            var command = new CreateRestaurantCommand();
            var restaurant = new Restaurant();

            mapperMock.Setup(m => m.Map<Restaurant>(command)).Returns(restaurant);

            var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

            restaurantRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Restaurant>()))
                                    .ReturnsAsync(1);

            var userContextMock = new Mock<IUserContext>();
            var currentUser = new CurrentUser("Owner-id", "test@test.com", [], null, null);

            userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);


            var commandHandler = new CreateRestaurantCommandHandler(loggerMock.Object, mapperMock.Object,
                restaurantRepositoryMock.Object, userContextMock.Object);

            //act 
            var result = await commandHandler.Handle(command, CancellationToken.None);


            // assert
            Assert.Equal(1, result);
            Assert.Equal("Owner-id", restaurant.OwnerId);
            restaurantRepositoryMock.Verify(r => r.CreateAsync(restaurant), Times.Once());
        }

    }

}
