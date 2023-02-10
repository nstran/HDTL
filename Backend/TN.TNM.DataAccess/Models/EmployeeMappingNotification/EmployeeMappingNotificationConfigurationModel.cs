using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.EmployeeMappingNotification
{
    public class EmployeeMappingNotificationConfigurationModel
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid NotificationConfigurationId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
