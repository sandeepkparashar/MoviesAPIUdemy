using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MoviesAPI.Helpers;
using MoviesAPI.Tests.IntegrationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    public class BaseTests
    {
        protected ApplicationDBContext BuildContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName).Options;
            var dbContext = new ApplicationDBContext(options);
            return dbContext;
        }

        protected IMapper BuildMap()
        {
            var config = new global::AutoMapper.MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfiles());
            }, new NullLoggerFactory());

            return config.CreateMapper();
        }
        protected WebApplicationFactory<Program> BuildWebApplicationFactory(string databaseName, bool bypassSecurity = true)
        {
            var factory = new WebApplicationFactory<Program>();

            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                //builder.ConfigureTestServices(services =>
                //{
                //    var descriptorDbContext = services.SingleOrDefault(d =>
                //    d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));
                //    if (descriptorDbContext != null)
                //    {
                //        services.Remove(descriptorDbContext);
                //    }
                //    services.AddDbContext<ApplicationDBContext>(options =>
                //    {
                //        options.UseInMemoryDatabase(databaseName);
                //    });
                //});
                if (bypassSecurity)
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
                        services.AddControllers(options =>
                        {
                            options.Filters.Add(new FakeActionFilter());
                        });
                    });
                }

            });

            return factory;
        }
    }
}
