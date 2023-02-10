using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Category;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataListSearchProductionOrderResponse : BaseResponse
    {
        public List<CategoryModel> ListStatus { get; set; }
        public List<OrganizationModel> Organizations { get; set; }
    }
}
