using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class FilterMovieTheatersDTO
    {
        [BindRequired]
        [Range(-90, 90)]
        public double Lat { get; set; }
        [BindRequired]
        [Range(-180, 180)]
        public double Long { get; set; }

        private int distnaceInKms = 10;
        private int maxDistanceInKms = 50;
        public int DistnaceInKms
        {
            get { return distnaceInKms; }
            set { distnaceInKms = (value > maxDistanceInKms) ? maxDistanceInKms : value; }
        }
    }
}
