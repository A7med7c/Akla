using FluentValidation.TestHelper;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;

namespace Restaurants.Domain.Test
{
    public class CreateRestaurantCommandValidatorTest
    {
        [Fact]
        public void Validator_ForValidCommand_ShoulldNotHaveValidationErrors()
        {
            // arrange
            var command = new CreateRestaurantCommand()
            {
                Name = "test",
                Category = "Italian",
                ContactEmail = "test@test.com",
                PostalCode = "12-345",
                Description = "New Description for a restaurant"
            };
            var validator = new CreateRestaurantCommandValidator();

            //act
            var result = validator.TestValidate(command);

            //assert 
            result.ShouldNotHaveAnyValidationErrors();
        }


        [Fact]
        public void Validator_ForInValidCommand_ShoulldHaveValidationErrors()
        {
            // arrange
            var command = new CreateRestaurantCommand()
            {
                Name = "te",
                Category = "Masri",
                ContactEmail = "@test.com",
                PostalCode = "12-345",
                Description = "New Description for a restaurant"
            };
            var validator = new CreateRestaurantCommandValidator();

            //act
            var result = validator.TestValidate(command);

            //assert 
            result.ShouldHaveValidationErrorFor(c => c.Name);
            result.ShouldHaveValidationErrorFor(c => c.Category);
            result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
        }
    }
}
