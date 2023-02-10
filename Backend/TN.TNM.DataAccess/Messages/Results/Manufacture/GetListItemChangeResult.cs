using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetListItemChangeResult : BaseResult
    {
        public List<ProductionOrderMappingEntityModel> ListItem { get; set; }
    }
}
