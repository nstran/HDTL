using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class TrackProductionModel : BaseModel<TrackProductionEntityModel>
    {
        public Guid ProductionOrderMappingId { get; set; }
        public Guid? ParentId { get; set; }
        public bool? ParentType { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public Guid ProductionOrderId { get; set; }
        public DateTime? EndDate { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }
        public double? ProductThickness { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string TechniqueDescription { get; set; }
        public Guid? StatusItemId { get; set; }
        public string StatusCode { get; set; }
        public string StatusItemName { get; set; }
        public Guid? OriginalId { get; set; }
        public byte? Rate { get; set; }
        public double? PreCompleteQuantity { get; set; }
        public double? Quantity { get; set; }
        public double? CompleteQuantity { get; set; }
        public double? UnitQuantity { get; set; }
        public double? ActionQuantity { get; set; }
        public bool IsShow { get; set; }
        public int TotalErrPre { get; set; }
        public byte? TechniqueOrder { get; set; }
        public bool IsParent { get; set; }
        public bool? IsSubParent { get; set; }
        public Guid? ParentPartId { get; set; }
        public int Type { get; set; }
        public Guid? ParentExtendId { get; set; }
        public int? Borehole { get; set; }
        public int? Hole { get; set; }
        public string ProductColor { get; set; }
        public bool? IsAddPart { get; set; }
        public string TextColorMode { get; set; }
        public string Note { get; set; }
        public bool? Present { get; set; }

        public List<double> ListRateChild { get; set; }
        public List<TechniqueRequestModel> ListTechniqueRequest { get; set; }

        public TrackProductionModel() { }

        public TrackProductionModel(TrackProductionEntityModel model)
        {
            Mapper(model, this);
            if (model.ListTechniqueRequest != null)
            {
                var cList = new List<TechniqueRequestModel>();
                model.ListTechniqueRequest.ForEach(child =>
                {
                    cList.Add(new TechniqueRequestModel(child));
                });
                this.ListTechniqueRequest = cList;
            }

            if (model.ListRateChild != null)
            {
                var cList = new List<double>();
                model.ListRateChild.ForEach(child =>
                {
                    cList.Add(child);
                });
                this.ListRateChild = cList;
            }
        }

        public override TrackProductionEntityModel ToEntity()
        {
            var entity = new TrackProductionEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
