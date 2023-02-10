using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetReportManuFactureByDayParameter: BaseParameter
    {
        public Guid? TechniqueRequestId { get; set; }
        public int? Shift { get; set; } //1: Ca ngày; 2: Ca đêm
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
