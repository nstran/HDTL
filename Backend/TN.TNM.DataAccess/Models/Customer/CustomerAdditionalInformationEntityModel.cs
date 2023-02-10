using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class CustomerAdditionalInformationEntityModel : BaseModel<DataAccess.Databases.Entities.CustomerAdditionalInformation>
    {
        public Guid CustomerAdditionalInformationId { get; set; }
        public Guid CustomerId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public CustomerAdditionalInformationEntityModel()
        {
        }

        public CustomerAdditionalInformationEntityModel(DataAccess.Databases.Entities.CustomerAdditionalInformation entity)
        {
            Mapper(entity, this);
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.CustomerAdditionalInformation ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.CustomerAdditionalInformation();
            Mapper(this, entity);
            return entity;
        }
    }
}
