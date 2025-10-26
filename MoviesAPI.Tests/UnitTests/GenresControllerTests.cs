using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class GenresControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetAllGenres()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            context.Genres.Add(new Entities.Genre { Name = "Genre1" });
            context.Genres.Add(new Entities.Genre { Name = "Genre2" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            var genreController = new GenresController(context2, mapper);
            var response = await genreController.Get();
            var genres = response.Value;
            Assert.AreEqual(2, genres?.Count);
        }

        [TestMethod]
        public async Task GetGenreByIdDoesNotExist()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            
            var genreController = new GenresController(context, mapper);
            int id = 1;
            var response = await genreController.Get(id);
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(404, result?.StatusCode);
        }

        [TestMethod]
        public async Task GetGenreByIdExist()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Entities.Genre { Name = "Genre 1" });
            context.Genres.Add(new Entities.Genre { Name = "Genre 2" });
            await context.SaveChangesAsync();
            Console.WriteLine($"Genres count before context2: {context.Genres.Count()}");
            var context2 = BuildContext(databaseName);

            var controller = new GenresController(context2, mapper);
            Console.WriteLine($"Genres count in context2: {context2.Genres.First().Id}");
            int id = 1;
            var response = await controller.Get(id);
            var result = (response.Result as OkObjectResult).Value as GenreDTO;

            Assert.AreEqual(id, result?.Id);
        }

        [TestMethod]
        public async Task CreateGenre()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var newGenre = new GenreCreationDTO() { Name = "New Genre" };
            
            var genreController = new GenresController(context, mapper);
            var response = await genreController.Post(newGenre);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result?.StatusCode);
            var context2 = BuildContext(databaseName);
            var count = await context2.Genres.CountAsync();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task UpdateGenre()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            context.Genres.Add(new Genre { Name="Genre 1"});
            await context.SaveChangesAsync();

            var context2 = BuildContext(databaseName);
            var mapper = BuildMap();
            var newGenre = new GenreCreationDTO() { Name = "New Name" };
            var genreController = new GenresController(context2, mapper);
            
            int id = 1;
            var response = await genreController.Put(id, newGenre);
            
            var result = response as NoContentResult;
            Assert.AreEqual(204, result?.StatusCode);
            
            var context3 = BuildContext(databaseName);
            var exists = await context3.Genres.AnyAsync(x => x.Name == "New Name");
            Assert.IsTrue(exists);
        }
    }
}
