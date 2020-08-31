using EmployeeManager.Angular.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Angular.Models
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
                
        }

      public  DbSet<Country> country { get; set; }
      public   DbSet<Employee> Employees { get; set; }
      public   DbSet<User> users { get; set; }


    }
}
