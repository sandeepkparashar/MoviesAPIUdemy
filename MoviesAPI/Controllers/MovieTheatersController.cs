using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/moviestheater")]
    public class MovieTheatersController:ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ApplicationDBContext applicationDBContext;

        public MovieTheatersController(IMapper mapper, ApplicationDBContext applicationDBContext)
        {
            this.mapper = mapper;
            this.applicationDBContext = applicationDBContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDTO>>> Get([FromQuery] FilterMovieTheatersDTO filterMovieTheatersDTO)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var userLocation = geometryFactory.CreatePoint(new Coordinate(filterMovieTheatersDTO.Long, filterMovieTheatersDTO.Lat));
            var theatres = await applicationDBContext.MovieTheaters
                .OrderBy(x => x.Location.Distance(userLocation))
                .Where(x => x.Location.IsWithinDistance(userLocation, filterMovieTheatersDTO.DistnaceInKms * 1000))
                .Select(x => new MovieTheaterDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    DistanceInMetres = Math.Round(x.Location.Distance(userLocation))
                })
                .ToListAsync();

            return theatres;
        }
    }
}
