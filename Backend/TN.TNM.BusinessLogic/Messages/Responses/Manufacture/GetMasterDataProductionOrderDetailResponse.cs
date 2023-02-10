using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataProductionOrderDetailResponse : BaseResponse
    {
        public ProductionOrderModel ProductionOrder { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
        public List<CategoryModel> ListStatusItem { get; set; }
        public List<NoteModel> ListNote { get; set; }
        public List<ProductionOrderMappingModel> ListProductItem { get; set; }
        public List<MappingOrderTechniqueModel> ListMappingOrder { get; set; }
        public List<TechniqueRequestModel> ListTechniqueRequest { get; set; }
    }
}
