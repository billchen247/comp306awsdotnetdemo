using System;
using System.ComponentModel.DataAnnotations;

namespace Example5WeekWeb.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Title { get; set; }

        public bool IsDone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
    }
}