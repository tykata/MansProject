using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EfectivoWork.Services;

var services = new ServiceCollection();
services.AddDbContext<Efectivo>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Efectivo;Trusted_Connection=True;MultipleActiveResultSets=true"));
services.AddSingleton<IApplication, Application>();
services.AddSingleton<ITaskManager, TaskManager>();
services.AddSingleton<NotificationService>();
services.AddSingleton<Efectivo>();

var serviceProvider = services.BuildServiceProvider();

var application = serviceProvider.GetService<IApplication>();
if (application != null)
{
    application.Run();
}
else
{
    // Obsłuż sytuację, gdy usługa IApplication nie jest zarejestrowana
}