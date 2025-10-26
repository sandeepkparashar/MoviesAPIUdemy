using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    [EnableCors("APIRequestIO")]
    public class GenresController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GenresController(ApplicationDBContext context,
            IMapper mapper): base(mapper, context)
        {
            this.context = context;
            this.mapper = mapper;
            
        }

        [HttpGet]
        
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            return await Get<Genre, GenreDTO>();
        }

        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> Get(int Id)
        {
            return await Get<Genre,GenreDTO>(Id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreation)
        {
            return await Post<Genre, GenreCreationDTO, GenreDTO>(genreCreation, "getGenre");
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var name=User.Identity.Name;
            }
            return await Delete<Genre>(id);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreation)
        {
            return await Put<Genre, GenreCreationDTO>(id, genreCreation);
        }
    }
}
