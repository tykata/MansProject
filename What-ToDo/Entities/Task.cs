using System.ComponentModel.DataAnnotations;

namespace EfectivoWork.Entities;

public class Task
{
    private string taskName;
    private DateTime now;

    [Key]
    public int TaskId { get; set; }
    public string Name { get; set; }
    public int EmployeeId { get; set; }
    public Pracownik Pracownik { get; set; } // Właściwość nawigacyjna
    public int Priority { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public List<Comment> Comment { get; set; }


    public Task(string name, int employeeId, int priority, DateTime dueDate)
    {
        Name = name;
        EmployeeId = employeeId;
        StartDate = DateTime.Now;
        Priority = priority;
        DueDate = dueDate;
        IsCompleted = false;
    }
}