using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetMasterDataListItemDialogParameter : BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
    }
}
