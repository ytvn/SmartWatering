using SmartWatering.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Models
{
    public class Variable : IEntityDate
    {
        [Key]
        public int VariableId { get; set; }
        [Display(Name = "Variable Name")]
        public string VariableName { get; set; }
        public int PinId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
