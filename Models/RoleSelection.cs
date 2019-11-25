using Lab5.Models.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab5.Models
{
    public class RoleSelection
    {
        public Role role { get; set; }
        public bool Selected { get; set; }

        public RoleSelection()
        {
            this.role = null;
            this.Selected = false;
        }

        public RoleSelection(Role role, bool Selected = false)
        {
            this.role = role;
            this.Selected = Selected;
        }
    }
}
