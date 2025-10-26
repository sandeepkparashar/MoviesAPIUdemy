using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class PeopleControllerTest : BaseTests
    {
        [TestMethod]
        public async Task GetPeoplePaginatedTest()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.People.Add(new Entities.Person { Name = "Person 1" });
            context.People.Add(new Entities.Person { Name = "Person 2" });
            context.People.Add(new Entities.Person { Name = "Person 3" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var controller = new PeopleController(context2, mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response1 = await controller.Get(new DTOs.PaginationDTO { Page = 1, RecordsPerPage = 2 });
            var result1 = response1.Value;
            Assert.AreEqual(2, result1?.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response2 = await controller.Get(new DTOs.PaginationDTO { Page = 2, RecordsPerPage = 2 });
            var result2 = response2.Value;
            Assert.AreEqual(1, result2?.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response3 = await controller.Get(new DTOs.PaginationDTO { Page = 3, RecordsPerPage = 2 });
            var result3 = response3.Value;
            Assert.AreEqual(0, result3?.Count);
        }

        [TestMethod]
        public async Task CreatePersonWithoutImage()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var personCreationDTO = new PersonCreationDTO { Name = "Person 1", Biography = "Test", DateOfBirth = DateTime.Now };
            var mock = new Mock<IFileStorageService>();
            mock.Setup(x => x.SaveFile(null, null, null, null))
                .Returns(Task.FromResult("url"));


            var controller = new PeopleController(context, mapper, mock.Object);
            var response = await controller.Post(personCreationDTO);
            var result = response as CreatedAtRouteResult;

            Assert.AreEqual(201, result?.StatusCode);

            var context2= BuildContext(databaseName);
            var people=await context2.People.ToListAsync();
            Assert.AreEqual(1, people.Count);
            Assert.IsNull(people[0].Picture);

            Assert.AreEqual(0, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CreatePersonWithImage()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var content = Encoding.UTF8.GetBytes("This is a test image");
            var file=new FormFile(new MemoryStream(content),0,content.Length,"Data","dummy.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";
            var personCreationDTO = new PersonCreationDTO { 
                Name = "Person 1", 
                Biography = "Test", 
                DateOfBirth = DateTime.Now,
                Picture=file
            };
            var mock = new Mock<IFileStorageService>();
            mock.Setup(x => x.SaveFile(content,".jpg","people", file.ContentType))
                .Returns(Task.FromResult("url"));

            var controller = new PeopleController(context, mapper, mock.Object);
            var response = await controller.Post(personCreationDTO);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result?.StatusCode);

            var context2 = BuildContext(databaseName);
            var people = await context2.People.ToListAsync();
            Assert.AreEqual(1, people.Count);
            Assert.AreEqual("url", people[0].Picture);

            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task PatchReturns404PersonNotFound()
        {
            var databaseName=Guid.NewGuid().ToString();
            var context=BuildContext(databaseName);
            var mapper = BuildMap();

            var controller = new PeopleController(context, mapper, null);
            var patchDoc = new JsonPatchDocument<PersonPatchDTO>();
            var response= await controller.Patch(1, patchDoc);
            var result= response as StatusCodeResult;
            Assert.AreEqual(404, result?.StatusCode);
        }        
    }
}
