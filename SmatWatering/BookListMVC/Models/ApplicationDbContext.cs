using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using SmartWatering.Models;
using SmartWatering.Interface;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookListMVC.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public override int SaveChanges()
        {
            var now = DateTime.Now;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is IEntityDate entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = now;
                            entity.UpdatedDate = now;
                            break;

                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                            entity.UpdatedDate = now;
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }

        public DbSet<BookListMVC.Models.Device> Device { get; set; }
        public DbSet<SmartWatering.Models.Schedule> Schedule { get; set; }
        public DbSet<BookListMVC.Models.DevicePin> DevicePIn { get; set; }
        public DbSet<BookListMVC.Models.Trigger> Trigger { get; set; }
        public DbSet<BookListMVC.Models.Variable> Variable { get; set; }
        public DbSet<BookListMVC.Models.VariableValue> VariableValue { get; set; }







    }
}
