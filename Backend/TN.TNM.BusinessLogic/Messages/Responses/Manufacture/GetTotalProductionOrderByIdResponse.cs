using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetTotalProductionOrderByIdResponse : BaseResponse
    {
        public TotalProductionOrderModel TotalProductionOrder { get; set; }
        public List<ProductionOrderModel> ListProductionOrder { get; set; }
        
        public List<CategoryModel> ListStatus { get; set; }
        public List<CategoryModel> ListStatusItem { get; set; }
    }
}
