using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetDataReportManufactureResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
    }
}
