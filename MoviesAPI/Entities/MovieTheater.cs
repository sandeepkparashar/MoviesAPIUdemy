using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class MovieTheater:IId
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public Point Location { get; set; }
    }
}
