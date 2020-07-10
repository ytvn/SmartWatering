using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;

namespace SmartWatering.Data
{
    public class SmartWateringContext : DbContext
    {
        public SmartWateringContext (DbContextOptions<SmartWateringContext> options)
            : base(options)
        {
        }

        public DbSet<BookListMVC.Models.VariableValue> VariableValue { get; set; }
    }
}
