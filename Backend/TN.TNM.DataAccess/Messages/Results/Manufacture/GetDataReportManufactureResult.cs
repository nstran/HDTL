using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetDataReportManufactureResult : BaseResult
    {
        public List<Models.Manufacture.TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
    }
}
