using MoviesAPI.Validator;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Person:IId
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Biography { get; set; }
        public string? Picture { get; set; }

        public List<MoviesActors> MoviesActors { get; set; }
    }
}
