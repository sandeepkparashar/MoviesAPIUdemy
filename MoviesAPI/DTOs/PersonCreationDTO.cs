using MoviesAPI.Validator;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class PersonCreationDTO:PersonPatchDTO
    {        
        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentType.Image)]
        public IFormFile? Picture { get; set; }
    }
}
