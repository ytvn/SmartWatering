using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Models.ViewModels
{
    public class VariableValueView
    {
        public int VariableValueId { get; set; }
        public float Value { get; set; }
        public string VariableName { get; set; }
        public string PIN { get; set; }
        public int ChipId { get; set; }
        public int VariableId { get; set; }
        public string CreatedDate { get; set; }
        public string  CreatedBy { get; set; }
    }
}
