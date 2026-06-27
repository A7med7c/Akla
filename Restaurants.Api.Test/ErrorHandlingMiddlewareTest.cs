using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Api.Middlewares;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Api.Test
{
    public class ErrorHandlingMiddlewareTest
    {
        private readonly Mock<ILogger<ErrorHandlingMiddleware>> loggerMock;
        private readonly ErrorHandlingMiddleware middleware;
        private readonly DefaultHttpContext context;

        public ErrorHandlingMiddlewareTest()
        {
            loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            context = new DefaultHttpContext();
        }
        [Fact]
        public async Task HandleInvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
        {

            var nextdelegateMock = new Mock<RequestDelegate>();

            await middleware.InvokeAsync(context, nextdelegateMock.Object);

            nextdelegateMock.Verify(next => next.Invoke(context), Times.Once());
        }

        [Fact]
        public async Task HandleInvokeAsync_WhenNotFoundExceptionThrown_StatusCodeShouldBe404()
        {

            var exception = new NotFoundException(nameof(Restaurant), "1");

            await middleware.InvokeAsync(context, _ => throw exception);

            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        }

        [Fact]
        public async Task HandleInvokeAsync_WhenForbiddenExceptionThrown_StatusCodeShouldBe403()
        {

            var exception = new ForbiddenException();

            await middleware.InvokeAsync(context, _ => throw exception);

            Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        }

        [Fact]
        public async Task HandleInvokeAsync_WhenGenericExceptionThrown_StatusCodeShouldBe500()
        {

            var exception = new Exception();

            await middleware.InvokeAsync(context, _ => throw exception);

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        }
    }
}
