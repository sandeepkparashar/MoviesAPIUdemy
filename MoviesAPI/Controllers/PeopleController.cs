using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MoviesAPI.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController(ApplicationDBContext context, IMapper mapper, IFileStorageService fileStorageService) : CustomBaseController(mapper, context)
    {
        private readonly ApplicationDBContext context = context;
        private readonly IMapper mapper = mapper;
        private readonly IFileStorageService fileStorageService = fileStorageService;
        private readonly string containerName = "people";

        // GET: api/<PeopleController>
        [HttpGet(Name ="getPeople")]
        [EnableCors("APIRequestIO")]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery]PaginationDTO pagination)
        {
            return await Get<Person,PersonDTO>(pagination);
        }

        // GET api/<PeopleController>/5
        [HttpGet("{id}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            return await Get<Person, PersonDTO>(id);
        }

        // POST api/<PeopleController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreationDTO)
        {
            var person = mapper.Map<Person>(personCreationDTO);
            if (personCreationDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.Picture.FileName);
                    person.Picture =
                        await fileStorageService.SaveFile(content, extension, containerName, personCreationDTO.Picture.ContentType);
                }
            }

            context.People.Add(person);
            await context.SaveChangesAsync();
            var personDTO = mapper.Map<PersonDTO>(person);
            return new CreatedAtRouteResult("getPerson", new { personDTO.Id }, personDTO);
        }

        // PUT api/<PeopleController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm] PersonCreationDTO person)
        {
            var personDB=await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if(personDB == null)
            {
                return NotFound();
            }
            personDB = mapper.Map(person, personDB);
            if (person.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await person.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(person.Picture.FileName);
                    personDB.Picture =
                        await fileStorageService.EditFile(content, extension, containerName,personDB.Picture, person.Picture.ContentType);
                }
            }
            await context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<PeopleController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Person>(id);
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            return await Patch<Person, PersonPatchDTO>(id, patchDocument);
        }
    }
    
}
