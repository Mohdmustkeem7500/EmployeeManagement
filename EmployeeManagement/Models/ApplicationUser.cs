using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }

        internal bool IsInRole(string v)
        {
            throw new NotImplementedException();
        }
    }
}
