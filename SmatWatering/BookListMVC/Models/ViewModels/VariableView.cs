using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Models.ViewModels
{
    public class VariableView
    {
        public int VariableId { get; set; }
        public string VariableName { get; set; }
        public string PIN { get; set; }
        public int ChipId { get; set; }
        public int  PinId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }





    }
}
