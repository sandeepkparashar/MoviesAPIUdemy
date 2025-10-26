
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class MoviesInTheatreService : IHostedService, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private Timer? timer;

        public MoviesInTheatreService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer=new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = serviceProvider.CreateScope()) { 
                var context=scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var moviesToBeReleased=await context.Movies.Where(x=>x.ReleaseDate==DateTime.Today).ToListAsync();
                if (moviesToBeReleased.Any())
                {
                    foreach (var movie in moviesToBeReleased)
                    {
                        movie.InTheatres = true;
                    }
                    await context.SaveChangesAsync();
                }
                
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
