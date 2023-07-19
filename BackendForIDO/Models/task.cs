using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BackendForIDO.Models
{
    public class TaskEntity
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the task

        [Required]
        public string Title { get; set; } // Title of the task (required)

        [Required]
        public bool Completed { get; set; } // Indicates whether the task is completed (required)

        public int? UserId { get; set; } // ID of the associated user (optional)

        public DateTime? CreatedAt { get; set; } // Date and time when the task was created (optional)

        public DateTime? UpdatedAt { get; set; } // Date and time when the task was last updated (optional)

        public string? Category { get; set; } // Add the Category property

        public DateTime? DueDate { get; set; } // Due date of the task (optional)

        public Estimate Estimate { get; set; } // Estimate of the task (optional)

        public string Importance { get; set; } // Importance of the task (low, medium, high)

        public string status {get; set;} // Add the status property

        public User? User { get; set; } // Navigation property for the related User (optional)
    }

    [Owned] // Specify that Estimate is an owned entity
     public class Estimate
    {
        public decimal Value { get; set; } // Numerical value of the estimate
        public string Units { get; set; } // Text for the units (e.g., hours, days, weeks)
    }
}
