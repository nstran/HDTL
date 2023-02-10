using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class BaoHiemLoftCareNhanSuModel
    {
        public string MaTheBhLoftCare { get; set; }
        public string NhomBhLoftCare { get; set; }
        public List<QuyenLoiBaoHiemLoftCareNhanSuModel> ListQuyenLoi { get; set; }

        public BaoHiemLoftCareNhanSuModel()
        {
            ListQuyenLoi = new List<QuyenLoiBaoHiemLoftCareNhanSuModel>();
        }
    }
}
