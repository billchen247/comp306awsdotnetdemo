using System;
using System.ComponentModel.DataAnnotations;

namespace Example6WeekMVCWebRDS.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string? Title { get; set; }

        // Matches SQL column name IsCompleted (BIT NOT NULL)
        public bool IsCompleted { get; set; }

        // Matches SQL column name DueAt (DATETIME2 NULL)
        [DataType(DataType.Date)]
        public DateTime? DueAt { get; set; }
    }
}
