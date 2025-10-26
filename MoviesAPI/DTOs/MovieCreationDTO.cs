
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Helpers;
using MoviesAPI.Validator;

namespace MoviesAPI.DTOs
{
    public class MovieCreationDTO:MoviePatchDTO
    {
        public DateTime ReleaseDate { get; set; }
        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentType.Image)]
        public IFormFile? Poster { get; set; }

        [ModelBinder(BinderType =typeof(TypeBinder<List<int>>))]
        public List<int>GenresIds { get; set; }

        [ModelBinder(BinderType =typeof(TypeBinder<List<ActorCreationDTO>>))]
        public List<ActorCreationDTO> Actors { get; set; }
    }
}
