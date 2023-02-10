using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class ImportLuongChiTietModel
    {
        public int KyLuongId { get; set; }
        public string EmployeeCode { get; set; }
        public decimal ThuNhapTinhThueNetToGross { get; set; }
        public decimal ThuNhapTinhThueMonth13 { get; set; }
        public decimal ThuNhapTinhThueGift { get; set; }
        public decimal ThuNhapTinhThueOther { get; set; }
        public decimal BaoHiemOther { get; set; }
        public decimal GiamTruTruocThueGiamTruKhac { get; set; }
        public decimal GiamTruSauThueQuyetToanThueTncn { get; set; }
        public decimal GiamTruSauThueOther { get; set; }
        public decimal HoanLaiSauThueThueTncn { get; set; }
        public decimal HoanLaiSauThueOther { get; set; }
        public decimal CtyDongOther { get; set; }
        public decimal CtyDongFundOct { get; set; }
        public decimal OtherKhoanBuTruThangTruoc { get; set; }
        public decimal OtherTroCapKhacKhongTinhThue { get; set; }
        public decimal OtherKhauTruHoanLaiTruocThue { get; set; }
        public decimal OtherLuongTamUng { get; set; }
    }
}
