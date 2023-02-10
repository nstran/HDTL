using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models
{
    public class RoleEntityModel
    {
        public Guid RoleId { get; set; }
        public string RoleValue { get; set; }
        public string Description { get; set; }
        public int UserNumber { get; set; }
    }
}
