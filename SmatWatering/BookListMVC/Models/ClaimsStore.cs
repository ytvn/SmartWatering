using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartWatering.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Edit All Role", "Edit All Role"),
            new Claim("Delete Role", "Delete Role"),
            new Claim("Delete All Role", "Delete All Role"),
            new Claim("Read Role", "Read Role"),
            new Claim("Read All Role", "Read All Role")
        };
    }
}
