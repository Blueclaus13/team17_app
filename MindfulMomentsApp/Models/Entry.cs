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

        // get mood color
        public string GetMoodColor()
        {
            return (Mood?.ToString() ?? "").ToLowerInvariant() switch
            {
                "sad" => "mood-sad",
                "angry" => "mood-angry",
                "mean" => "mood-angry",
                "joy" => "mood-happy",
                "excitement" => "mood-happy",
                "happy" => "mood-happy",
                "fear" => "mood-fear",
                "nervious" => "mood-fear",
                "afraid" => "mood-fear",
                "anxious" => "mood-anxiety",
                "stress" => "mood-anxiety",
                "exhaustion" => "mood-tired",
                "tired" => "mood-tired",
                "shy" => "mood-shy",
                "surprise" => "mood-surprise",
                "boredom" => "mood-bored",
                "inlove" => "mood-love",
                "peace" => "mood-peace",
                "calm" => "mood-peace",
                "serenity" => "mood-peace",
                "disgust" => "mood-disgust",
                _ => "mood-default"

            };
        }
    }
}
