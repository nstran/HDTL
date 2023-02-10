using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
   public  class SearchQuanlityControlReportResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.QuanlityControlReportModel> ListQuanlityControlReport { get; set; }
        public List<DataAccess.Models.Manufacture.TechniqueRequestReportModel> ListTechniqueRequestReport { get; set; }
    }
}
