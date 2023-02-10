using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetThongTinBaoHiemVaThueResult : BaseResult
    {
        public BaoHiemXaHoiNhanSuModel BaoHiemXaHoi { get; set; }
        public BaoHiemLoftCareNhanSuModel BaoHiemLoftCare { get; set; }
        public ThongTinThueVaGiamTruModel ThongTinThueVaGiamTru { get; set; }
        public List<CategoryEntityModel> ListDoiTuongPhuThuoc { get; set; }
        public List<LichSuThanhToanBaoHiemModel> ListLichSuThanhToanBaoHiem { get; set; }
        public List<BaseType> ListLoaiBaoHiem { get; set; }
        public bool IsShowButton { get; set; }
    }
}
