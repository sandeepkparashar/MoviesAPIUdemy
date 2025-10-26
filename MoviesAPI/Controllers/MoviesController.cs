using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Linq.Dynamic.Core;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController(ApplicationDBContext context, IMapper mapper, IFileStorageService fileStorageService) : CustomBaseController(mapper,context)
    {
        private readonly ApplicationDBContext context = context;
        private readonly IMapper mapper = mapper;
        private readonly IFileStorageService fileStorageService = fileStorageService;
        //private readonly ILogger logger;
        private readonly string containerName = "movies";

        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = context.Movies.AsQueryable();
            await HttpContext.InsertPaginationHeaderInResponse(queryable, pagination.RecordsPerPage);
            var movies = await queryable.Paginate(pagination).AsNoTracking().ToListAsync();
            var movieDTOs = mapper.Map<List<MovieDTO>>(movies);
            return Ok(movieDTOs);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Get([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var queryable= context.Movies.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                queryable = queryable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }
            if (filterMoviesDTO.InTheatres.HasValue)
            {
                queryable = queryable.Where(x => x.InTheatres);
            }
            if (filterMoviesDTO.UpcomingReleases.HasValue)
            {
                queryable = queryable.Where(x => x.ReleaseDate > DateTime.Today);
            }

            if (filterMoviesDTO.GenreId != 0)
            {
                queryable = queryable.Where(x => x.MoviesGenres
                    .Select(x => x.GenreId)
                    .Contains(filterMoviesDTO.GenreId));
            }

            try
            {
                queryable = queryable.OrderBy($"{filterMoviesDTO.OrderingField} {(filterMoviesDTO.ascendingOrder ? "ascending" : "descending")}");
            }
            catch {
                //string message = $"Invalid field {filterMoviesDTO.OrderingField} for ordering.";
                //logger.Log(LogLevel.Warning, message);
            }
            await HttpContext.InsertPaginationHeaderInResponse(queryable, filterMoviesDTO.RecordsPerPage);
            var movies=await queryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }

       [HttpGet("latest")]
        public async Task<ActionResult<IndexMovieDTO>> GetLatest()
        {
            var top = 6;
            var upcomingReleases = await context.Movies.Where(x => x.ReleaseDate > DateTime.Today)
                .Take(top)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();
            var inTheatre=await context.Movies.Where(x=>x.InTheatres==true)
                .Take(top)
                .OrderByDescending(x=>x.ReleaseDate)
                .ToListAsync();
            var result = new IndexMovieDTO();
            result.InTheatres=mapper.Map<List<MovieDTO>>(inTheatre);
            result.UpcomingReleases = mapper.Map<List<MovieDTO>>(upcomingReleases);
            
            return result;
        }

        [HttpGet("{id}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            var queryable = context.Movies
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .AsQueryable();
                        
            return await Get<Movie,MovieDetailsDTO>(id,queryable);
        }
       

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO);
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movie.Poster =
                        await fileStorageService.SaveFile(content, extension, containerName, movieCreationDTO.Poster.ContentType);
                }
            }
            AnnotateActorsOrder(movie);
            context.Movies.Add(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("getMovie", new { movieDTO.Id }, movieDTO);
        }

        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movie)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieDB == null)
            {
                return NotFound();
            }
            movieDB = mapper.Map(movie, movieDB);
            if (movie.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movie.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movie.Poster.FileName);
                    movieDB.Poster =
                        await fileStorageService.EditFile(content, extension, containerName, movieDB.Poster, movie.Poster.ContentType);
                }
            }
            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId={movieDB.Id}; delete from MoviesGenres where MovieId={movieDB.Id};");
            AnnotateActorsOrder(movieDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Movie>(id);
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            return await Patch<Movie, MoviePatchDTO>(id, patchDocument);
        }
    }
}
