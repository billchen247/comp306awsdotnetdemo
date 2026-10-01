using System.ComponentModel.DataAnnotations;

namespace Example4Week4RDS.Data
{
    // Simple Todo model used by the sample API and EF Core.
    // Fields:
    // - Id: primary key, identity column
    // - Title: short description of the todo item (required)
    // - IsCompleted: indicates whether the task is done
    // - DueAt: optional due date/time
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime? DueAt { get; set; }
    }
}
