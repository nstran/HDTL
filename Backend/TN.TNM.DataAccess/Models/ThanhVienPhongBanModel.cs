using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models
{
    public class ThanhVienPhongBanModel
    {
        public Guid? Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public int IsManager { get; set; }
        public string OrganizationName { get; set; }
        public string EmployeeCodeName { get; set; }
    }
}
