using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class SearchQuanlityControlReportParameter: BaseParameter
    {
        public List<Guid?> ListTechniqueRequestId { get; set; }
        public List<int?> ListThicknessOptionId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
