using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class UpdateKhauHaoMobileParameter: BaseParameter
    {
        public int TaiSanId { get; set; }
        public decimal? GiaTriNguyenGia { get; set; }
        public decimal? GiaTriTinhKhauHao { get; set; }
        public int? ThoiGianKhauHao { get; set; }
        public DateTime? ThoiDiemDatDauTinhKhauHao { get; set; }
    }
}
