using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class OrderTechniqueMappingModel : BaseModel<OrderTechniqueMappingEntityModel>
    {
        public Guid OrderTechniqueMappingId { get; set; }
        public Guid ProductOrderWorkflowId { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public byte? TechniqueOrder { get; set; }
        public byte? Rate { get; set; }
        public bool? IsDefault { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string TechniqueName { get; set; }

        public OrderTechniqueMappingModel() { }

        public OrderTechniqueMappingModel(OrderTechniqueMappingEntityModel model)
        {
            Mapper(model, this);
        }

        public override OrderTechniqueMappingEntityModel ToEntity()
        {
            var entity = new OrderTechniqueMappingEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
