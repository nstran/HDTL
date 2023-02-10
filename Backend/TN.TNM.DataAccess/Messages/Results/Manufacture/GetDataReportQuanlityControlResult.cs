using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetDataReportQuanlityControlResult : BaseResult
    {
        public List<Models.Manufacture.TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListQualityControlNote { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListErrorType { get; set; }
    }
}
