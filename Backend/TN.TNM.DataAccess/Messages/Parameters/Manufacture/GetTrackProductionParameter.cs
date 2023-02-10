using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetTrackProductionParameter : BaseParameter
    {
        public List<Guid> ListProductionOrderId { get; set; }
        public DateTime? EndDate { get; set; }
        public double? ProductThickness { get; set; }
        public string ProductName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid> ListStatusItem { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }
        public int FirstNumber { get; set; }
        public int Rows { get; set; }
    }
}
