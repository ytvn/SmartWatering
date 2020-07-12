using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Models.ViewModels
{
    public class VariableView
    {
        [Display(Name = "Variable ID")]
        public int VariableId { get; set; }
        [Display(Name = "Variable Name")]
        public string VariableName { get; set; }
        public string PIN { get; set; }
        [Display(Name = "Chip ID")]
        public int ChipId { get; set; }
        [Display(Name = "PIN ID")]
        public int  PinId { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }





    }
}
