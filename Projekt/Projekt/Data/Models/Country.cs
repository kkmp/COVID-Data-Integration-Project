using System;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Data.Models
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Slug { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
