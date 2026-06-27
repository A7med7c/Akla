using Restaurants.Domain.Entities;

namespace Restaurants.Application.Payments;

public interface IPaymentService
{
    Task<string> CreatePaymentAsync(Order order);
}
