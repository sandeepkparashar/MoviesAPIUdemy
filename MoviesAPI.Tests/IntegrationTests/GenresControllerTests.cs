using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Tests.UnitTests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.IntegrationTests
{
    [TestClass]
    public class GenresControllerTests:BaseTests
    {
        [TestMethod]
        public async Task GetAllGenresEmptyList()
        {
            var databaseName=Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);

            var client=factory.CreateClient();
            var url = "api/genres";
            var response= await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genres=JsonConvert.DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(0,genres?.Count);

        }
        [TestMethod]
        public async Task GetAllGenres()
        {
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);
            var context = BuildContext("TestingDB");
            context.Genres.Add(new Entities.Genre { Name = "Genre 1" });
            context.Genres.Add(new Entities.Genre { Name = "Genre 2" });
            context.SaveChanges();


            var client = factory.CreateClient();
            var url = "api/genres";
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genres = JsonConvert.DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(2, genres?.Count);
        }

        [TestMethod]
        public async Task DeleteGenres()
        {
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);
            var context = BuildContext("TestingDB");
            context.Genres.Add(new Entities.Genre { Name = "Genre 1" });
            context.SaveChanges();

            var client = factory.CreateClient();
            var url = "/api/genres";
            var response = await client.DeleteAsync($"{url}/1");

            response.EnsureSuccessStatusCode();

            var context2=BuildContext(databaseName);
            var exists=await context2.Genres.AnyAsync();
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public async Task DeleteGenresShouldBeProtected()
        {
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName,false);
            var context = BuildContext("TestingDB");
            context.Genres.Add(new Entities.Genre { Name = "Genre 1" });
            context.SaveChanges();

            var client = factory.CreateClient();
            var url = "/api/genres";
            var response = await client.DeleteAsync($"{url}/1");

            Assert.AreEqual("Unauthorized",response.ReasonPhrase);
        }
    }
}
