using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.AuditTrace;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class TraceModel : BaseModel<AuditTrace>
    {
        public Guid UserId { get; set; }

        public Guid? EmployeeId { get; set; }

        public string UserName { get; set; }

        public string EmployeeName { get; set; }

        public string ActionName { get; set; }

        public string ActionType { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }
        
        public string ObjectCode { get; set; }

        public DateTime CreateDate { get; set; }
        
        public string Description { get; set; }

        public string BackgroundColor { get; set; }

        public string Color { get; set; }


        public TraceModel() { }

        public TraceModel(TraceEntityModel entity)
        {
            Mapper(entity, this);
        }

        public TraceModel(AuditTrace entity) : base(entity)
        {

        }
        

        public override AuditTrace ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new AuditTrace();
            Mapper(this, entity);
            return entity;
        }
    }
}
