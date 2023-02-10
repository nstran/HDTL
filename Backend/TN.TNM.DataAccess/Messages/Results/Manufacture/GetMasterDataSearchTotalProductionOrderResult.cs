using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataSearchTotalProductionOrderResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
    }
}
