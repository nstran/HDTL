using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetThongTinLuongVaTroCapResult : BaseResult
    {
        public decimal LuongHienTai { get; set; }
        public decimal MucLuongKyHopDong { get; set; }
        public List<TroCapNhanSuModel> ListTroCap { get; set; }
    }
}
