using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class CustomerCareCustomerEntityModel:BaseModel<CustomerCareCustomer>
    {
        public Guid CustomerCareCustomerId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public CustomerCareCustomerEntityModel()
        {

        }

        public CustomerCareCustomerEntityModel(CustomerCareCustomer entity)
        {
            Mapper(entity, this);
        }

        public override CustomerCareCustomer ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new CustomerCareCustomer();
            Mapper(this, entity);
            return entity;
        }
    }
}
