using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfectivoWork.Entities;
public class Pracownik
{
    [Key]
    public int EmployeeId { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }

    [NotMapped]
    public string PelneImie
    {
        get { return $"{Name} {LastName}"; }
    }
    public List<Task> Tasks { get; set; }
}
