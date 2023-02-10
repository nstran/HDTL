using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Models.PermissionConfiguration
{
    public class PermissionConfigurationEntityModel
    {
        public Guid Id { get; set; }
        public int? StepId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool? IsEdit { get; set; }
        public string CategoryCode { get; set; }
    }
}
