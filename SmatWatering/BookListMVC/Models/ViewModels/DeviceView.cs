using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Models.ViewModels
{
    public class DeviceView
    {
        [Display(Name = "Chip ID")]
        public int ChipId { get; set; }
        public string Description { get; set; }
    }
}
