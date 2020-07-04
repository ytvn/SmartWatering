using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Interface
{
    interface IEntityDate
    {
        DateTime CreatedDate { get; set; }

        DateTime UpdatedDate { get; set; }
    }
}
