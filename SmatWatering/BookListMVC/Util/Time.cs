using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Util
{
    public class Time
    {
        [Key]
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
