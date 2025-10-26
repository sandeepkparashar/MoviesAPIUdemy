namespace MoviesAPI.DTOs
{
    public class MovieTheaterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double DistanceInMetres { get; set; }
        public double DistanceInKms { get { return DistanceInMetres / 1000; } }
   
    }
}
