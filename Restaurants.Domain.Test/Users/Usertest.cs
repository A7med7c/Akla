using Restaurants.Application.Users;
using Restaurants.Domain.Constants;

namespace Restaurants.Domain.Test.Users
{
    public class Usertest
    {
        [Fact]
        public void IsInRole_WithMatchingRole_ShouldReturnTrue()
        {
            var user = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User, UserRoles.Owner, UserRoles.Driver], null, null);

            var result = user.IsInRole(UserRoles.Admin);

            Assert.True(result);
        }

        [Fact]
        public void IsInRole_WithNoMatchingRole_ShouldReturnFalse()
        {
            var user = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User, UserRoles.Owner], null, null);

            var result = user.IsInRole(UserRoles.Driver);

            Assert.False(result);
        }

        [Fact]
        public void IsInRole_WithNoMatchingRoleCase_ShouldReturnFalse()
        {
            var user = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User, UserRoles.Owner], null, null);

            var result = user.IsInRole(UserRoles.Admin.ToLower());

            Assert.False(result);
        }


        [Theory]
        [InlineData(UserRoles.Admin)]
        [InlineData(UserRoles.Owner)]
        public void IsInRole_WithMatchingRoles_ShouldReturnTrue(string roleName)
        {
            var user = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.Owner], null, null);

            var result = user.IsInRole(roleName);

            Assert.True(result);
        }

    }
}
