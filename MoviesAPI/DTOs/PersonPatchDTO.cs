using MoviesAPI.Validator;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class PersonPatchDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Biography { get; set; }        
    }
}
