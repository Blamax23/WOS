using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WOS.Dal.Interfaces;

public class DailyTaskService : BackgroundService
{

    private readonly IGlobalDataSrv _globalDataSrv;

    public DailyTaskService(IGlobalDataSrv globalDataSrv)
    {
        _globalDataSrv = globalDataSrv;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = now.Date.AddDays(1).AddHours(0); // Minuit demain

            // Temps restant avant la prochaine exécution
            var delay = nextRun - now;

            await Task.Delay(delay, stoppingToken); // Attends jusqu'à minuit

            try
            {
                // Exécute ta tâche quotidienne ici
                await RunDailyTask();
            }
            catch (Exception ex)
            {
                // Gère les exceptions
                Console.WriteLine($"Erreur dans la tâche quotidienne : {ex.Message}");
            }
        }
    }

    private Task RunDailyTask()
    {
        Console.WriteLine($"Tâche quotidienne exécutée à {DateTime.Now}");
       
        _globalDataSrv.RefreshAllCacheAsync();

        return Task.CompletedTask;
    }
}