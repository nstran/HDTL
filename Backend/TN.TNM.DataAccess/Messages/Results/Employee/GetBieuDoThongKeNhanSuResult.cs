using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetBieuDoThongKeNhanSuResult : BaseResult
    {
        public List<String> CategoriesChart { get; set; }
        public List<ThongKeNhanSuChartModel> ChartThongKeNhanSu { get; set; }
        public List<DataPieChartModel> PieChartThongKeNhanSu { get; set; }
    }
}
