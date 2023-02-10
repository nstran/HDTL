using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetMasterDataViewRememberItemDialogParameter : BaseParameter
    {
        public string ProductionOrderCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ProductName { get; set; }
        public double? ProductThickness { get; set; }
    }
}
