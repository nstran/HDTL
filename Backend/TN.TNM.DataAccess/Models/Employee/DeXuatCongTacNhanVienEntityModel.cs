using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeXuatCongTacNhanVienEntityModel
    {  
        public Guid EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }      
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdentityID { get; set; }        
        public Guid? TenantId { get; set; }
    }
}
