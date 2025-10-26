using MoviesAPI.Validator;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class GenreCreationDTO
    {
        public int Id { get; set; }
        [Required]
        [FirstLetterUppercase]
        [StringLength(40, ErrorMessage = "Too long name!")]
        public string Name { get; set; }
    }
}
