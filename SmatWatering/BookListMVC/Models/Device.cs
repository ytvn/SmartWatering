using SmartWatering.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Models
{
    public class Device :IEntityDate
    {
        [Key]
        public int DeviceId { get; set; }
        public string Name { get; set; }
        [Display(Name = "Chip ID")]
        public int ChipId { get; set; }
        public string Description { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string  ReadAPIKey { get; set; }
        public string  WriteAPIKey { get; set; }

    }
}
