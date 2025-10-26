using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MoviesAPI.Tests.IntegrationTests
{
    public class FakeActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Email, "example@hotmail.com")
                }, "Test"));
            await next();
        }
    }
}
