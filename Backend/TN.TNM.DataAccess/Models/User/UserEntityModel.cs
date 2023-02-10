using System;

namespace TN.TNM.DataAccess.Models.User
{
    public class UserEntityModel
    {
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Disabled { get; set; }
        public DateTime? ResetCodeDate { get; set; }
        public string ResetCode { get; set; }
        public bool? IsAdmin { get; set; }
        public string DeviceId { get; set; }
    }
}
