using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class TechniqueRequestModel : BaseModel<TechniqueRequestEntityModel>
    {
        public Guid TechniqueRequestId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string TechniqueName { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public byte? Rate { get; set; }
        public byte? TechniqueOrder { get; set; }
        public double? TechniqueValue { get; set; }
        public string TechniqueRequestCode { get; set; }

        public string ParentName { get; set; }
        public string OrganizationName { get; set; }
        public double? CompleteUnitQuantity { get; set; }
        public double? CompleteAreaInDay { get; set; }

        public TechniqueRequestModel() { }

        public TechniqueRequestModel(TechniqueRequestEntityModel model)
        {
            Mapper(model, this);
        }

        public override TechniqueRequestEntityModel ToEntity()
        {
            var entity = new TechniqueRequestEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
