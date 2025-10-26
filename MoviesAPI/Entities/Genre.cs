using MoviesAPI.Validator;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Genre:IId
    {
        public int Id { get; set; }
        [Required]
        [FirstLetterUppercase]
        [StringLength(40,ErrorMessage = "Too long name!")]
        public string Name { get; set; }
        public List<MoviesGenres>MoviesGenres  { get; set; }
    }
}
