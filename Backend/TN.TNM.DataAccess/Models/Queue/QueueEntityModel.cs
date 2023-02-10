using System;

namespace TN.TNM.DataAccess.Models.Queue
{
    public class QueueEntityModel : BaseModel<Databases.Entities.Queue>
    {
        public Guid QueueId { get; set; }
        public string SendTo { get; set; }
        public string SendContent { get; set; }
        public string Title { get; set; }
        public Guid Method { get; set; }
        public bool? IsSend { get; set; }
        public DateTime? SenDate { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public string FromTo { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CustomerMeetingId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        public QueueEntityModel()
        {
            
        }
        public QueueEntityModel(Databases.Entities.Queue entity)
        {
            Mapper(entity, this);
        }

        public override Databases.Entities.Queue ToEntity()
        {
            var entity = new Databases.Entities.Queue();
            Mapper(this, entity);
            return entity;
        }
    }
}
