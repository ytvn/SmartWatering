using SmartWatering.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Models
{
    public class RequestValue 
    {
        public int ID{ get; set; }
        public float Value { get; set; }
        public string Token { get; set; }


    }
}
