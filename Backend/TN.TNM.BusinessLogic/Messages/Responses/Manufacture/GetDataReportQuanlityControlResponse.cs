using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetDataReportQuanlityControlResponse: BaseResponse
    {
        public List<Models.Manufacture.TechniqueRequestModel> ListTechniqueRequest { get; set; }
        public List<Models.Category.CategoryModel> ListQualityControlNote { get; set; }
        public List<Models.Category.CategoryModel> ListErrorType { get; set; }
    }
}
