using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetDataDasboardManufactureResponse: BaseResponse
    {
        public List<TechniqueRequestModel> ListTechniqueRequest { get; set; }
        public List<DataAccess.Models.Manufacture.DelayProductionOrderInDasboardEntityModel> ListDelayProductionOrder { get; set; }
        public double TotalCompleteArea { get; set; }
        public double TotalArea { get; set; }
        public List<ProductionOrderInDayModel> ListProductionOrderInDay { get; set; }
        public DataAccess.Models.Manufacture.SumaryDashboardEntityModel SumaryDashboard { get; set; }
    }
}
