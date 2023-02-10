using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.EmployeeMappingNotification;

namespace TN.TNM.DataAccess.Models.PermissionConfiguration
{
    public class NotificationConfigurationEntityModel
    {
        public Guid Id { get; set; }
        public Guid? ServicePacketId { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? ServiceRequestMaker { get; set; }
        public bool? ServiceManagement { get; set; }
        public bool? ServiceSupporter { get; set; }
        public bool? Supporter { get; set; }
        public bool? Reporter { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? SortOrder { get; set; }
        public string CategoryName { get; set; }
        public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
    }
}
