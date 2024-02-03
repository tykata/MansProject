using Spectre.Console;



namespace EfectivoWork.Services;

public interface IApplication
{
    void Run();
}

public class Application : IApplication
{
    private readonly ITaskManager _taskManager;
    private readonly NotificationService _notificationService;

    public Application(ITaskManager taskManager, NotificationService notificationService)
    {
        _taskManager = taskManager;
        _notificationService = notificationService;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();

            _taskManager.LoadTasks();
            _taskManager.DisplayTasks();
            _taskManager.DisplayMeetings();
            _notificationService.SendNotifications();
            HandleSelection();

        }
    }
    public string DisplayMenu()
    {
        var rule = new Rule();
        rule.LeftJustified();
        AnsiConsole.Write(rule);

        var options = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices("Add Task", "Edit Task", "Mark Task as completed", "Delete Task", "Add Meeting", "Edit Meeting", "Delete Meeting", "Display Meetings", "Sort Tasks", "Sort Meetings", "Add Comment to Task", "Generate Report", "Exit") // Dodajemy nową opcję "Generate Report"
        );

        return options;
    }

        public void HandleSelection()
    {

        var selection = DisplayMenu();
        switch (selection)
        {
            case "Add Task":
                _taskManager.AddTask();
                break;

            case "Edit Task":
                _taskManager.EditTask();
                break;

            case "Mark Task as completed":
                _taskManager.MarkTaskAsCompleted();
                break;

            case "Delete Task":
                _taskManager.DeleteTask();
                break;
            case "Add Meeting":
                _taskManager.AddMeeting();
                break;

            case "Edit Meeting":
                _taskManager.EditMeeting();
                break;

            case "Delete Meeting":
                _taskManager.DeleteMeeting();
                break;

            case "Display Meetings":
                _taskManager.DisplayMeetings();
                break;

            case "Exit":
                Environment.Exit(0);
                break;
            case "Sort Tasks":
                var taskSortBy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Sort tasks by:")
                        .AddChoices("date", "priority", "status")
                );
                _taskManager.FilterAndSortTasks(null, taskSortBy);
                break;

            case "Sort Meetings":
                var meetingSortBy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Sort meetings by:")
                        .AddChoices("date", "participants")
                );
                _taskManager.FilterAndSortMeetings(null, meetingSortBy);
                break;

            case "Add Comment to Task":
                var taskName = AnsiConsole.Ask<string>("Enter the name of the task you want to comment on: ");
                var notes = AnsiConsole.Ask<string>("Enter your comment: ");
                _taskManager.AddCommentToTask(taskName, notes);
                break;

            case "Generate Report":
                _taskManager.GenerateReport();
                _taskManager.DisplayReport();
                break;
        }
    }
}
