using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataAddProductionOrderDialogResult : BaseResult
    {
        public List<ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
