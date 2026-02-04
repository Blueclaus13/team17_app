using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using MindfulMomentsApp.Models.Enums;

namespace MindfulMomentsApp.Models;

public class Entry
{
    public int Id { get; set; }
    
    [Display(Name = "Date")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public Moods Mood { get; set; }
     [Required]
    public Activities Activity { get; set; }

    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Description { get; set; }
}