using Microsoft.EntityFrameworkCore;
using EfectivoWork.Entities;

public class Efectivo : DbContext
{
    public Efectivo(DbContextOptions<Efectivo> options)
    : base(options)
    { }

    public DbSet<Pracownik> Employee { get; set; }
    public DbSet<Comment> Comment { get; set; }

    public DbSet<EfectivoWork.Entities.Task> Tasks { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfectivoWork.Entities.Task>()
            .HasOne(t => t.Pracownik)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.EmployeeId);

          modelBuilder.Entity<Comment>()
         .HasOne(c => c.Task)
         .WithMany(t => t.Comment)
        .HasForeignKey(c => c.TaskId);

        modelBuilder.Entity<Comment>()
            .Property(c => c.CommentId)
            .ValueGeneratedOnAdd();

    }
}

