using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SaveThongTinBaoHiemVaThueParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public BaoHiemXaHoiNhanSuModel BaoHiemXaHoi { get; set; }
        public BaoHiemLoftCareNhanSuModel BaoHiemLoftCare { get; set; }
        public ThongTinThueVaGiamTruModel ThongTinThueVaGiamTru { get; set; }
    }
}
