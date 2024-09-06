using System;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Data.Models
{
    public class DailyCase
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Country Country { get; set; }

        [Required]
        public int NewConfirmed { get; set; }

        [Required]
        public int NewDeaths { get; set; }

        [Required]
        public int NewRecovered { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
