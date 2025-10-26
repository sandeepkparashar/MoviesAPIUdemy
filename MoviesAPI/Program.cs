
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Text;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //When running tests, tests will set env to "Testing"
            if (builder.Environment.EnvironmentName == "Testing")
            {
                builder.Services.AddDbContext<ApplicationDBContext>(options =>
                    options.UseInMemoryDatabase("TestingDB"));
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDBContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultString"),
                        sqlServer => sqlServer.UseNetTopologySuite()));
            }


            // Add services to the container.
            //builder.Services.AddDbContext<ApplicationDBContext>(options => 
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultString")
            //    ,sqlServer=>sqlServer.UseNetTopologySuite()));

            builder.Services.AddCors(
                options=>
                {
                    options.AddPolicy("APIRequestIO",
                        x => x.WithOrigins("https://apirequest.io").WithMethods("GET", "POST").AllowAnyHeader()
                        );
                });

            builder.Services.AddAutoMapper(cfg=>cfg.AddProfile(typeof(AutoMapperProfiles)));
            builder.Services.AddTransient<IFileStorageService,InAppStorageService>();
            //builder.Services.AddTransient<IHostedService,MoviesInTheatreService>();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])),
                    ClockSkew = TimeSpan.Zero
                }
                );

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();
            //app.UseCors(x => x.WithOrigins("https://apirequest.io").WithMethods("GET", "POST").AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
