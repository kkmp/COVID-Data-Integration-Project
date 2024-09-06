using Client.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Data.Models
{
    public class DailyCase
    {
        public Guid Id { get; set; }

        public Country? Country { get; set; }

        public int NewConfirmed { get; set; }

        public int NewDeaths { get; set; }

        public int NewRecovered { get; set; }

        public DateTime? Date { get; set; }
    }
}
