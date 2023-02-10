using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetMasterDataDialogListProductOrderParameter:BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
    }
}
