using System.Threading.Tasks;
using BiblionegaBot.Anounces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BiblionegaBot
{
    internal class Program
    {
        private static void ConfigureServices(ServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(s => new Worker(
                    s.GetService<ILogger<Worker>>(), 
                    s.GetService<IAnounceParser>(), 
                    s.GetService<IDataLayer>(),
                    configuration))
                .AddSingleton<IAnounceParser>(s => new AnounceParser(
                    configuration["SiteAddress"], 
                    configuration["AnouncesPath"], 
                    s.GetService<ILogger<AnounceParser>>()))
                .AddSingleton<IDataLayer>(s => new DataLayer(
                    s.GetService<ILogger<DataLayer>>(), 
                    configuration["Database"]));

            services.AddLogging(loggingBuilder => {
                var loggingSection = configuration.GetSection("Logging");
                loggingBuilder.AddFile(loggingSection);
            });

        }

        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            Worker app = serviceProvider.GetService<Worker>();
            var silent = false;
            if (args.Length > 0)
            {
                bool.TryParse(args[0], out silent);
            }
            
            await app.RunAsync(silent).ConfigureAwait(false);
        }
    }
}
