using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ThongTinThueVaGiamTruModel
    {
        public string MaSoThueCaNhan { get; set; }
        public decimal? SoNguoiDangKyPhuThuoc { get; set; }
        public DateTime? ThangNopDangKyGiamTru { get; set; }
        public List<Guid?> ListDoiTuongPhuThuocId { get; set; }
    }
}
