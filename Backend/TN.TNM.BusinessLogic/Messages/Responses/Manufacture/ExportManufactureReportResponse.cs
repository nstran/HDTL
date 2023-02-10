using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class ExportManufactureReportResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.TechniqueRequestReportModel> ListTechniqueRequestReport { get; set; }
    }
}
