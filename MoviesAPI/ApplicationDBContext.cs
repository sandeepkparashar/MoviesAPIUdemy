using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesAPI
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
        public DbSet<MovieTheater> MovieTheaters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.GenreId, x.MovieId });
            modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.MovieId, x.PersonId });
            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            modelBuilder.Entity<MovieTheater>()
                .HasData(new List<MovieTheater>
                {
                new () {Id=1, Name="Agora", Location=geometryFactory.CreatePoint(new Coordinate(-69.9388777,18.4839233))},
                new (){Id=2, Name="Sambil", Location=geometryFactory.CreatePoint(new Coordinate(-69.9118804,18.4826214))},
                new (){Id=3, Name="Megacentro", Location=geometryFactory.CreatePoint(new Coordinate(-69.856427,18.506934))},
                new (){Id=4, Name="Village East Cinema", Location=geometryFactory.CreatePoint(new Coordinate(-73.986227,40.730898))}
                });

            var adventure = new Genre { Id = 4, Name = "Adventure" };
            var animation = new Genre { Id = 5, Name = "Animation" };
            var drama = new Genre { Id = 6, Name = "Drama" };
            var romance = new Genre { Id = 7, Name = "Romance" };

            modelBuilder.Entity<Genre>()
                .HasData(new List<Genre> { adventure, animation, drama, romance });

            var jimCarrey = new Person { Id = 9, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 01, 17) };
            var robertDowney = new Person { Id = 10, Name = "Robert Dawney Jr.", DateOfBirth = new DateTime(1965, 4, 4) };
            var chrisEvans = new Person { Id = 11, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };
            modelBuilder.Entity<Person>()
                .HasData(new List<Person> { jimCarrey, robertDowney, chrisEvans });

            var endGame = new Movie
            {
                Id = 4,
                Title = "Avengers:Endgame",
                InTheatres = false,
                ReleaseDate = new DateTime(2019, 04, 26)
            };
            var iw = new Movie
            {
                Id = 5,
                Title = "Avengers:Infinity War",
                InTheatres = false,
                ReleaseDate = new DateTime(2018, 04, 27)
            };

            var sonic = new Movie
            {
                Id = 6,
                Title = "Sonic The Hedgehog",
                InTheatres = false,
                ReleaseDate = new DateTime(2020, 02, 28)
            };
            var emma = new Movie
            {
                Id = 7,
                Title = "Emma",
                InTheatres = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };

            var greed = new Movie
            {
                Id = 8,
                Title = "Greed",
                InTheatres = false,
                ReleaseDate = new DateTime(2019, 09, 7)
            };

            modelBuilder.Entity<Movie>()
                .HasData(new List<Movie>() { endGame, iw, sonic, emma, greed });

            modelBuilder.Entity<MoviesGenres>().HasData(
                new List<MoviesGenres>()
                {
                    new MoviesGenres() {MovieId=endGame.Id, GenreId=drama.Id },
                    new MoviesGenres() { MovieId = endGame.Id, GenreId = adventure.Id },
                    new MoviesGenres() { MovieId = iw.Id, GenreId = drama.Id },
                    new MoviesGenres() { MovieId = iw.Id, GenreId = adventure.Id },
                    new MoviesGenres() { MovieId = sonic.Id, GenreId = drama.Id },
                    new MoviesGenres() { MovieId = emma.Id, GenreId = drama.Id },
                    new MoviesGenres() { MovieId = emma.Id, GenreId = romance.Id },
                    new MoviesGenres() { MovieId = greed.Id, GenreId = drama.Id },
                    new MoviesGenres() { MovieId = greed.Id, GenreId = romance.Id }
                });

            modelBuilder.Entity<MoviesActors>().HasData(
                new List<MoviesActors>()
                {
                    new MoviesActors{MovieId=endGame.Id, PersonId=robertDowney.Id, Character="Tony Starc", Order=1},
                    new MoviesActors{MovieId=endGame.Id, PersonId=chrisEvans.Id, Character="Steve Rogers", Order=2},
                    new MoviesActors{MovieId=iw.Id, PersonId=robertDowney.Id, Character="Tony Starc", Order=1},
                    new MoviesActors{MovieId=iw.Id, PersonId=chrisEvans.Id, Character="Steve Rogers", Order=2},
                    new MoviesActors{MovieId=sonic.Id, PersonId=jimCarrey.Id, Character="Dr. Ivo Robotnik", Order=1}
                });

        }
        protected ApplicationDBContext()
        {
        }
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
    }
}
