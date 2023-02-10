using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class MappingOrderTechniqueModel : BaseModel<MappingOrderTechniqueEntityModel>
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public string Name { get; set; }
        public bool? IsDefault { get; set; }
        public Guid? ParentId { get; set; }

        public List<TechniqueRequestModel> ListTechniqueRequest { get; set; }

        public MappingOrderTechniqueModel() { }

        public MappingOrderTechniqueModel(MappingOrderTechniqueEntityModel model)
        {
            Mapper(model, this);
        }

        public override MappingOrderTechniqueEntityModel ToEntity()
        {
            var entity = new MappingOrderTechniqueEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
