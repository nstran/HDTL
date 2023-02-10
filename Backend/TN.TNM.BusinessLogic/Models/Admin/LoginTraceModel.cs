using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.AuditTrace;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class LoginTraceModel : BaseModel<LoginAuditTrace>
    {
        public Guid UserId { get; set; }

        public Guid? EmployeeId { get; set; }

        public string UserName { get; set; }

        public DateTime? LoginDate { get; set; }
        
        public int? StatusCode { get; set; } 

        public string BackgroundColor { get; set; }

        public string Color { get; set; }
        
        public string EmployeeCode { get; set; }
        
        public string EmployeeName { get; set; }

        public LoginTraceModel() { }

        public LoginTraceModel(LoginTraceEntityModel entity)
        {
            Mapper(entity, this);
        }

        public LoginTraceModel(LoginAuditTrace entity) : base(entity)
        {

        }

        public override LoginAuditTrace ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.LoginAuditTrace();
            Mapper(this, entity);
            return entity;
        }
    }
}
