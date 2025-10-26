
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class MoviePatchDTO
    {
        [Required]
        [StringLength(300)]
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public bool InTheatres { get; set; }
        public DateTime ReleaseDate { get; set; }        

    }
}
