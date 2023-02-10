using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetDataDashboardHomeResult : BaseResult
    {
        public List<DataPieChartModel> DataThongKeNhanSu { get; set; }
        public List<DataPieChartModel> DataThongKeNhanSuSapHetHanHD { get; set; }
        public List<DataPieChartModel> DataThongKeNhanSuSapHetHanThuViec { get; set; }
        public List<ThongKeTaiSanChartModel> DataThongKeTaiSan { get; set; }
        public List<String> ListTenTaiSan { get; set; }
        public List<DataPieChartModel> DataThongKeNhanSuSinhNhat { get; set; }
    }
}
