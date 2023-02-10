using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class SearchTotalProductionOrderResult : BaseResult
    {
        public List<TotalProductionOrderEntityModel> ListTotalProductionOrder { get; set; }
        public int TotalRecords { get; set; }
    }
}
