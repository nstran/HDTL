using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataCreateTotalProductionOrderResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
