using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System.Runtime.CompilerServices;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();
            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<PersonCreationDTO, Person>()
                .ForMember(x=>x.Picture,options=>options.Ignore());
            CreateMap<Person,PersonPatchDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x=>x.MoviesGenres, options=>options.MapFrom(MapMoviesGenres))
                .ForMember(x=>x.MoviesActors, options=>options.MapFrom(MapMoviesActors));

            CreateMap<Movie,MovieDetailsDTO>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMovieDetailsGenres))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMovieDetailsActors));

            CreateMap<Movie, MoviePatchDTO>().ReverseMap();

            CreateMap<IdentityUser, UserDTO>()
                .ForMember(x => x.UserId, options => options.MapFrom(x => x.Id))
                .ForMember(x => x.EmailAddress, opttions => opttions.MapFrom(x => x.Email));
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();
            foreach (var id in movieCreationDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach(var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { PersonId = actor.PersonId, Character=actor.Character });
            }
            return result;
        }

        private List<GenreDTO> MapMovieDetailsGenres( Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<GenreDTO>();
            foreach (var movieGenre in movie.MoviesGenres)
            {
                result.Add(new GenreDTO() { Id = movieGenre.GenreId,Name=movieGenre.Genre.Name });
            }
            return result;
        }

        private List<ActorDTO> MapMovieDetailsActors( Movie movie, MovieDetailsDTO movieCreationDTO)
        {
            var result = new List<ActorDTO>();
            foreach (var movieActor in movie.MoviesActors)
            {
                result.Add(new ActorDTO() { PersonId = movieActor.PersonId, Character = movieActor.Character, ActorName=movieActor.Person.Name });
            }
            return result;
        }
    }
}
