using MindfulMomentsApp.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MindfulMomentsApp.Models
{
    public class Entry
    {
        public int EntryId { get; set; }
        public int JournalId { get; set; }
        [ValidateNever]
        public Journal? Journal { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        [Required(ErrorMessage = "Please select a mood.")]
        public Mood? Mood { get; set; }
        [Required(ErrorMessage = "Please select an activity.")]
        public Activity? Activity { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
