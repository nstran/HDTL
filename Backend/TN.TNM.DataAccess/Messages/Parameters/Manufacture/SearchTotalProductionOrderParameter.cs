using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class SearchTotalProductionOrderParameter : BaseParameter
    {
        public string Code { get; set; }
        public DateTime? StartDate { get; set; }
        public double? TotalQuantity { get; set; }
        public double? TotalArea { get; set; }
        public DateTime? MinEndDate { get; set; }
        public DateTime? MaxEndDate { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public int FirstNumber { get; set; }
        public int Rows { get; set; }
    }
}
