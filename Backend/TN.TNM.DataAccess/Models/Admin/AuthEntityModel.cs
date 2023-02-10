using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Admin
{
    public class AuthEntityModel
    {
        public string Token { get; set; }
        public DateTime LoginTime { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? PositionId { get; set; }
    }
}
