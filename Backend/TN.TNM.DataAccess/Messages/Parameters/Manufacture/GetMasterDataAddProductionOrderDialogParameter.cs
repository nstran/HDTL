using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetMasterDataAddProductionOrderDialogParameter : BaseParameter
    {
        public List<Guid> ListIgnore { get; set; }
    }
}
