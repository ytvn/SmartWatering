using SmartWatering.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Models
{
    public class DevicePin : IEntityDate
    {
        [Key]
        public int PinId { get; set; }
        [Display(Name = "PIN Description")]
        public string Description { get; set; }
        public string PIN { get; set; }
        [Display(Name = "Chip ID")]
        public int chipId { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; }
    }
}
