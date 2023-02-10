using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataTrackProductionResponse : BaseResponse
    {
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string CurrentTime { get; set; }
        public List<CategoryModel> ListStatusItem { get; set; }
        public List<ProductionOrderModel> ListProductionOrder { get; set; }
    }
}
