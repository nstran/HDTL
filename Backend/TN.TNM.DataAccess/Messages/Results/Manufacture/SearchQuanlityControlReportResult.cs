using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class SearchQuanlityControlReportResult: BaseResult
    {
        public List<Models.Manufacture.QuanlityControlReportModel> ListQuanlityControlReport { get; set; }
        public List<Models.Manufacture.TechniqueRequestReportModel> ListTechniqueRequestReport { get; set; }
    }
}
