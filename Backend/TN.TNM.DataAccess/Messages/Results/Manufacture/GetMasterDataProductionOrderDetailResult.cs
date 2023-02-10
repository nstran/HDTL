using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Manufacture;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataProductionOrderDetailResult : BaseResult
    {
        public ProductionOrderEntityModel ProductionOrder { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<CategoryEntityModel> ListStatusItem { get; set; }
        public List<ProductionOrderMappingEntityModel> ListProductItem { get; set; }
        public List<MappingOrderTechniqueEntityModel> ListMappingOrder { get; set; }
        public List<TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }

    }
}
