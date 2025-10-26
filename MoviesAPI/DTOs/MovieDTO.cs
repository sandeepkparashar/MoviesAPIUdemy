
namespace MoviesAPI.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Summary { get; set; }
        public bool InTheatres { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster { get; set; }

    }
}
