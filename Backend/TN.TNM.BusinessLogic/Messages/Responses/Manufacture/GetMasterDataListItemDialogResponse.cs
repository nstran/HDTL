using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataListItemDialogResponse : BaseResponse
    {
        public List<ProductionOrderMappingModel> ListItem { get; set; }
    }
}
