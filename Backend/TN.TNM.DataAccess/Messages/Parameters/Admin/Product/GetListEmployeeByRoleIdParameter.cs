using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.PermissionConfiguration;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ServicePacketImage;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class GetListEmployeeByRoleIdParameter : BaseParameter
    {
        public Guid RoleId { get; set; }
    }
}
