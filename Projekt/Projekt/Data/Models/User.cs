using System;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Data.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }
    }
}
