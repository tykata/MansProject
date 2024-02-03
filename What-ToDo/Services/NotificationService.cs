using Spectre.Console;
using EfectivoWork.Services;

public class NotificationService
{
    private readonly ITaskManager _taskManager;

    public NotificationService(ITaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public void SendNotifications()
    {
        var upcomingTasks = _taskManager.GetUpcomingTasks();
        var upcomingMeetings = _taskManager.GetUpcomingMeetings();

        if (upcomingTasks.Any())
        {
            AnsiConsole.WriteLine($"You have {upcomingTasks.Count} upcoming tasks.");
        }

        if (upcomingMeetings.Any())
        {
            AnsiConsole.WriteLine($"You have {upcomingMeetings.Count} upcoming meetings.");
        }
    }
}
