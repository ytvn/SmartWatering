using SmartWatering.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Models
{
    public class Trigger : IEntityDate
    {
        //quan ly tren dau esp8266/ardruino/ , moi esp sẽ có n PIN, mỗi PIN gắn liền với mỗi sensor, 
        [Key]
        public int Id { get; set; }
        [Display(Name = "Variable Name")]
        public int VariableId { get; set; }
        public string  Description { get; set; }
        [Display(Name = "Min")]
        public float min { get; set; }
        [Display(Name = "Max")]
        public float max { get; set; }
        public int Interval { get; set; }
        [Display(Name = "Duration (second)")]
        public int Duration { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; }
    }

}
