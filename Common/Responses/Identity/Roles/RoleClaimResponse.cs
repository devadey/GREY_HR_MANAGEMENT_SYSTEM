using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Responses.Identity.Roles
{
    public class RoleClaimResponse
    {
        public RoleResponse Role { get; set; }
        public List<RoleClaimViewModel> RoleClaim { get; set; }
    }
}
