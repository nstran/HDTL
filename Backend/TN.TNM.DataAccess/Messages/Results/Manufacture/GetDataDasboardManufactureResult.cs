using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetDataDasboardManufactureResult: BaseResult
    {
        public List<TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
        public List<DelayProductionOrderInDasboardEntityModel> ListDelayProductionOrder { get; set; }
        public double TotalCompleteArea { get; set; } //Tổng số m2 đã hoàn thành của các lệnh sản xuất chậm tiến độ
        public double TotalArea { get; set; }   //Tổng số m2 phải hoàn thành của các lệnh sản xuất chậm tiến độ
        public List<ProductionOrderInDayEntityModel> ListProductionOrderInDay { get; set; }
        public SumaryDashboardEntityModel SumaryDashboard { get; set; }
    }
}
