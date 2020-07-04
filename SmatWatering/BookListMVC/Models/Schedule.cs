using SmartWatering.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Interval type")]
        public IntervalType intervalType { get; set; }
        [Display(Name = "Times start")]
        public TimeSpan TimeStart { get; set; }
        public int Interval { get; set; }
        public string Device { get; set; }
        public string PIN { get; set; }
        public string Variable { get; set; }

    }
}
