using Spectre.Console;
using EfectivoWork.Entities;
using TaskEntity = EfectivoWork.Entities.Task;

namespace EfectivoWork.Services;

public interface ITaskManager
{
    void AddTask();
    void EditTask();
    void MarkTaskAsCompleted();
    void DeleteTask();
    void LoadTasks();
    void DisplayTasks();
    void AddMeeting();
    void EditMeeting();
    void DeleteMeeting();
    void DisplayMeetings();
    void FilterAndSortTasks(string filterBy, string sortBy);
    void FilterAndSortMeetings(string filterBy, string sortBy);
    List<TaskEntity> GetUpcomingTasks();
    List<Meeting> GetUpcomingMeetings();
    void GenerateReport();

    void AddCommentToTask(string taskName, string notes);

    void DisplayReport();
}

    public class TaskManager : ITaskManager
    {
        private List<TaskEntity> tasks = new();
        private Efectivo db; 

        public TaskManager(Efectivo db) 
    {
        this.db = db;
    }

    public List<TaskEntity> GetUpcomingTasks()
        {
        var now = DateTime.Now;
        return tasks.Where(task => task.DueDate > now).ToList();
    }

        public List<Meeting> GetUpcomingMeetings()
        {
            var now = DateTime.Now;
        return meetings.Where(meeting => meeting.StartTime > now).ToList();
        }

    public void AddTask()
    {
        while (true)
        {
            var taskName = AnsiConsole.Ask<string>("Enter task name (Enter 0 to return): ");
            var employees = db.Employee.ToList();
            if (employees.Count == 0)
            {
                AnsiConsole.WriteLine("No employees found in the database.");
                return;
            }
            var employee = AnsiConsole.Prompt(new SelectionPrompt<Pracownik>()
                .AddChoices(employees)
                .UseConverter(p => p.PelneImie));
            var priority = AnsiConsole.Ask<int>("Enter task priority (Enter 0 to return): ");
            var dueDate = AnsiConsole.Ask<DateTime>("Enter due date (yyyy-mm-dd) (Enter 0 to return): ");
            if (taskName == "0" || employee == null)
            {
                return;
            }
            var task = new Entities.Task(taskName, employee.EmployeeId, priority, dueDate); // Używamy wybranego pracownika
            db.Tasks.Add(task);

             db.SaveChanges();

           
            tasks.Add(task); // Dodajemy zadanie do lokalnej listy
            UpdateTasks();
            break;
        }
    }


    public void EditTask()
    {
        if (tasks.Count == 0)
        {
            return;
        }

        var prompt = new SelectionPrompt<string>()
            .Title("Select a task to edit:");

        foreach (var task in tasks)
        {
            prompt.AddChoice(task.Name);
        }

        var selectedTaskName = AnsiConsole.Prompt(prompt);
        var selectedTask = tasks.First(task => task.Name == selectedTaskName);

        var newTaskName = AnsiConsole.Ask<string>($"Enter a new name for the task '{selectedTaskName}' (Enter 0 to return): ");
        if (newTaskName == "0")
        {
            return;
        }

        var employees = db.Employee.ToList();
        if (employees.Count == 0)
        {
            AnsiConsole.WriteLine("No employees found in the database.");
            return;
        }
        var employee = AnsiConsole.Prompt(new SelectionPrompt<Pracownik>()
            .AddChoices(employees)
            .UseConverter(p => p.PelneImie));

        selectedTask.Name = newTaskName;
        selectedTask.EmployeeId = employee.EmployeeId; // Uaktualniamy pracownika przypisanego do zadania
        UpdateTasks();
    }


    public void MarkTaskAsCompleted()
    {
        if (tasks.Count == 0)
        {
            return;
        }

        var selectedTask = AnsiConsole.Prompt(
            new SelectionPrompt<Entities.Task>()
                .Title("Select a task to mark as completed:")
                .AddChoices(tasks)
                .UseConverter(task => task.Name)
        );

        selectedTask.IsCompleted = true;
        UpdateTasks();
    }

    public void DeleteTask()
    {
        if (tasks.Count == 0)
        {
            return;
        }

        var selectedTask = AnsiConsole.Prompt(
            new SelectionPrompt<Entities.Task>()
                .Title("Select a task to delete:")
                .AddChoices(tasks)
                .UseConverter(task => task.Name)
        );

        tasks.Remove(selectedTask);
        UpdateTasks();
    }
    public void AddCommentToTask(string taskName, string notes)
    {
        if (tasks.Count == 0)
        {
            AnsiConsole.WriteLine("No tasks to comment on.");
            return;
        }

        Console.WriteLine($"Looking for task with name: {taskName}"); 

        var selectedTask = db.Tasks.FirstOrDefault(t => t.Name == taskName);
        if (selectedTask == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        var comment = new Comment
        {
            TaskId = selectedTask.TaskId,
            Notes = notes,
            RecordStamp = DateTime.Now
        };

        db.Comment.Add(comment);
        db.SaveChanges();
    }



    public void UpdateTasks()
    {
        File.WriteAllLines("tasks.txt", tasks.Select(task => $"{task.Name}|{task.EmployeeId}|{task.Priority}|{task.DueDate.ToString("yyyy-MM-dd")}|{task.IsCompleted}"));
    }

    public void LoadTasks()
    {
        if (File.Exists("tasks.txt"))
        {
            var lines = File.ReadAllLines("tasks.txt");
            tasks = new List<Entities.Task>();

            foreach (var line in lines)
            {
                try
                {
                    var parts = line.Split('|');
                    var task = new Entities.Task(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), DateTime.Parse(parts[3])) { IsCompleted = bool.Parse(parts[4]) };
                    tasks.Add(task);
                }
                catch (FormatException ex)
                {
                    AnsiConsole.WriteLine($"Error parsing line: '{line}'. Parts: {string.Join(", ", line.Split('|'))}");
                    AnsiConsole.WriteException(ex);
                }
            }
        }
    }

    public void DisplayTasks()
    {
        AnsiConsole.WriteLine("EfetivoWork");

        var table = new Table();
        table.AddColumn("Task ID");
        table.AddColumn("Task");
        table.AddColumn("Employee");
        table.AddColumn("Priority");
        table.AddColumn("Due Date");
        table.AddColumn("Status");

        foreach (var task in tasks)
        {
            var dbTask = db.Tasks.FirstOrDefault(t => t.Name == task.Name);
            if (dbTask != null)
            {
                var employee = db.Employee.Find(dbTask.EmployeeId); // Pobieramy pracownika
                var employeeName = employee != null ? employee.PelneImie : "Unknown"; // Jeśli pracownik nie istnieje, ustawiamy imię i nazwisko na "Unknown"

                
                table.AddRow(dbTask.TaskId.ToString(), task.Name, employeeName, task.Priority.ToString(), task.DueDate.ToString("yyyy-MM-dd"), task.IsCompleted ? "Completed" : "Not completed");
            }
        }
        AnsiConsole.Write(table);
    }


    private List<Meeting> meetings = new();

    public void AddMeeting()
    {
        while (true)
        {
            var topic = AnsiConsole.Ask<string>("Enter meeting topic (Enter 0 to return): ");
            var employees = db.Employee.ToList();
            if (employees.Count == 0)
            {
                AnsiConsole.WriteLine("No employees found in the database.");
                return;
            }
            var selectedEmployees = AnsiConsole.Prompt(new MultiSelectionPrompt<Pracownik>()
                .AddChoices(employees)
                .UseConverter(p => p.PelneImie));
            var location = AnsiConsole.Ask<string>("Enter meeting location: ");
            var startTime = AnsiConsole.Ask<DateTime>("Enter meeting start time (yyyy-mm-dd hh:mm): ");
            var duration = AnsiConsole.Ask<TimeSpan>("Enter meeting duration (hh:mm): ");

            if (topic == "0" || !selectedEmployees.Any())
            {
                return;
            }

            var participants = selectedEmployees.Select(e => e.PelneImie).ToList();
            var meeting = new Meeting(topic, participants, location, startTime, duration);
            meetings.Add(meeting);
            break;
        }
    }


    public void EditMeeting()
    {
        if (meetings.Count == 0)
        {
            AnsiConsole.WriteLine("No meetings to edit.");
            return;
        }

        var prompt = new SelectionPrompt<string>()
            .Title("Select a meeting to edit:")
            .AddChoices(meetings.Select(m => m.Topic));

        var selectedMeetingTopic = AnsiConsole.Prompt(prompt);
        var selectedMeeting = meetings.First(m => m.Topic == selectedMeetingTopic);

        var newTopic = AnsiConsole.Ask<string>($"Enter a new topic for the meeting '{selectedMeetingTopic}' (Enter 0 to return): ");
        if (newTopic == "0") return;
        selectedMeeting.Topic = newTopic;

        var employees = db.Employee.ToList();
        if (employees.Count == 0)
        {
            AnsiConsole.WriteLine("No employees found in the database.");
            return;
        }
        var selectedEmployees = AnsiConsole.Prompt(new MultiSelectionPrompt<Pracownik>()
            .AddChoices(employees)
            .UseConverter(p => p.PelneImie));
        if (!selectedEmployees.Any()) return;
        selectedMeeting.Participants = selectedEmployees.Select(e => e.PelneImie).ToList();

        var newLocation = AnsiConsole.Ask<string>($"Enter a new location for the meeting '{selectedMeetingTopic}' (Enter 0 to return): ");
        if (newLocation == "0") return;
        selectedMeeting.Location = newLocation;

        var newStartTime = AnsiConsole.Ask<DateTime>($"Enter a new start time for the meeting '{selectedMeetingTopic}' (yyyy-mm-dd hh:mm) (Enter 0 to return): ");
        if (newStartTime == DateTime.MinValue) return;
        selectedMeeting.StartTime = newStartTime;

        var newDuration = AnsiConsole.Ask<TimeSpan>($"Enter a new duration for the meeting '{selectedMeetingTopic}' (hh:mm) (Enter 0 to return): ");
        if (newDuration == TimeSpan.Zero) return;
        selectedMeeting.Duration = newDuration;
    }

    public void DeleteMeeting()
    {
        if (meetings.Count == 0)
        {
            AnsiConsole.WriteLine("No meetings to delete.");
            return;
        }

        var prompt = new SelectionPrompt<string>()
            .Title("Select a meeting to delete:")
            .AddChoices(meetings.Select(m => m.Topic));

        var selectedMeetingTopic = AnsiConsole.Prompt(prompt);
        var selectedMeeting = meetings.First(m => m.Topic == selectedMeetingTopic);

        meetings.Remove(selectedMeeting);
    }
    public void DisplayMeetings()
    {
        AnsiConsole.WriteLine("Meetings:");

        var table = new Table();
        table.AddColumn("Topic");
        table.AddColumn("Participants");
        table.AddColumn("Location");
        table.AddColumn("Start Time");
        table.AddColumn("Duration");

        foreach (var meeting in meetings)
        {
            table.AddRow(meeting.Topic, string.Join(", ", meeting.Participants), meeting.Location, meeting.StartTime.ToString("yyyy-MM-dd HH:mm"), meeting.Duration.ToString());
        }
        AnsiConsole.Write(table);
    }

    public void FilterAndSortTasks(string filterBy, string sortBy)
    {
        var filteredTasks = tasks;

        switch (filterBy)
        {
            case "date":
                filteredTasks = tasks.Where(t => t.DueDate.Date == DateTime.Today).ToList();
                break;
            case "priority":
                filteredTasks = tasks.Where(t => t.Priority == 1).ToList(); 
                break;
            case "status":
                filteredTasks = tasks.Where(t => t.IsCompleted).ToList();
                break;
        }
        switch (sortBy)
        {
            case "date":
                filteredTasks = filteredTasks.OrderBy(t => t.DueDate).ToList();
                break;
            case "priority":
                filteredTasks = filteredTasks.OrderBy(t => t.Priority).ToList();
                break;
            case "status":
                filteredTasks = filteredTasks.OrderBy(t => t.IsCompleted).ToList();
                break;
        }
        foreach (var task in filteredTasks)
        {
            AnsiConsole.WriteLine($"{task.Name} {task.EmployeeId} {task.Priority} {task.DueDate} {task.IsCompleted}");
        }
    }

    public void FilterAndSortMeetings(string filterBy, string sortBy)
    {
        var filteredMeetings = meetings;

        switch (filterBy)
        {
            case "date":
                filteredMeetings = meetings.Where(m => m.StartTime.Date == DateTime.Today).ToList();
                break;
            case "participants":
                filteredMeetings = meetings.Where(m => m.Participants.Count > 1).ToList(); // Filter by meetings with more than one participant
                break;
        }

        switch (sortBy)
        {
            case "date":
                filteredMeetings = filteredMeetings.OrderBy(m => m.StartTime).ToList();
                break;
            case "participants":
                filteredMeetings = filteredMeetings.OrderBy(m => m.Participants.Count).ToList();
                break;
        }

        foreach (var meeting in filteredMeetings)
        {
            AnsiConsole.WriteLine($"{meeting.Topic} {string.Join(", ", meeting.Participants)} {meeting.Location} {meeting.Duration}");
        }
    }
    public void GenerateReport()
    {
        var tasks = db.Tasks.ToList();
        var comments = db.Comment.ToList();

        using (var writer = new StreamWriter("report.txt"))
        {
            writer.WriteLine("Tasks:");
            foreach (var task in tasks)
            {
                var taskInfo = $"Task ID: {task.TaskId}, Name: {task.Name}, Employee ID: {task.EmployeeId}, Priority: {task.Priority}, Due Date: {task.DueDate}, Is Completed: {task.IsCompleted}";
                writer.WriteLine(taskInfo);
                AnsiConsole.WriteLine(taskInfo);
            }

            if (comments.Any()) 
            {
                writer.WriteLine("\nComments:");
                foreach (var comment in comments)
                {
                    var commentInfo = $"Comment ID: {comment.CommentId}, Task ID: {comment.TaskId}, Notes: {comment.Notes}, Record Stamp: {comment.RecordStamp}";
                    writer.WriteLine(commentInfo);
                    AnsiConsole.WriteLine(commentInfo);
                }
            }
        }

        AnsiConsole.WriteLine("Report generated successfully.");
    }
    public void DisplayReport()
    {
        var report = File.ReadAllText("report.txt");
        AnsiConsole.WriteLine(report);
    }



}




