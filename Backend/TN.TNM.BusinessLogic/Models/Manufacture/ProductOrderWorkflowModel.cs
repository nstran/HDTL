using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class ProductOrderWorkflowModel : BaseModel<ProductOrderWorkflowEntityModel>
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsDefault { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public ProductOrderWorkflowModel() { }

        public ProductOrderWorkflowModel(ProductOrderWorkflowEntityModel model)
        {
            Mapper(model, this);
        }

        public override ProductOrderWorkflowEntityModel ToEntity()
        {
            var entity = new ProductOrderWorkflowEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
