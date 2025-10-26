using Microsoft.AspNetCore.Hosting;
namespace MoviesAPI.Services
{
    public class WriteToFileHostedService : IHostedService
    {
        private readonly IWebHostEnvironment environment;
        private readonly string FileName = "File 1.txt";
        private Timer timer;
        public WriteToFileHostedService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process started.");
            timer=new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process stopped.");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            WriteToFile($"Process ongoing:{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}");
        }
        private void WriteToFile(string message)
        {
            var path = $@"{environment.ContentRootPath}\wwwroot\{FileName}";
            using(StreamWriter sw=new StreamWriter(path, append:true))
            {
                sw.WriteLine(message);
            }
        }
    }
}
