using Api_Orbis_Project.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Models
{
    public class Guide
    {
        [Key]
        public int GuideId { get; set; }

        public string Country { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public GuideCategory Category { get; set; }

        public ICollection<Trip> Trips { get; set; }

        public enum GuideCategory
        {
            Health,
            Culture,
            Safety
        }
    }
}
