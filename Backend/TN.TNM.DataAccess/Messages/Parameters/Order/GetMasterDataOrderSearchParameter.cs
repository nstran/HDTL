using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetMasterDataOrderSearchParameter : BaseParameter
    {
        public bool IsOrderAction { get; set; }
        public bool IsOrderProcess { get; set; }
    }
}
