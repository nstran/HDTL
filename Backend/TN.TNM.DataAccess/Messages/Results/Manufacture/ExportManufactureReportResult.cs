using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class ExportManufactureReportResult: BaseResult
    {
        public List<Models.Manufacture.TechniqueRequestReportModel> ListTechniqueRequestReport { get; set; }
    }
}
