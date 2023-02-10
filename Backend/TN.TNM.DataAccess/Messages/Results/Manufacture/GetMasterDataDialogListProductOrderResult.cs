using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataDialogListProductOrderResult : BaseResult
    {
        public List<ProductionOrderMappingEntityModel> ListProduct { get; set; }
    }
}
