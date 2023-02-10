using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class CustomerCareFeedBackEntityModel: BaseModel<CustomerCareFeedBack>
    {
        public CustomerCareFeedBackEntityModel(CustomerCareFeedBack customerCareFeedBack)
        {
            Mapper(customerCareFeedBack, this);
        }

        public Guid CustomerCareFeedBackId { get; set; }
        public DateTime? FeedBackFromDate { get; set; }
        public DateTime? FeedBackToDate { get; set; }
        public Guid? FeedbackType { get; set; }
        public Guid? FeedBackCode { get; set; }
        public string FeedBackContent { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }

        public CustomerCareFeedBackEntityModel()
        {

        }

        public override CustomerCareFeedBack ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new CustomerCareFeedBack();
            Mapper(this, entity);
            return entity;
        }
    }
}
