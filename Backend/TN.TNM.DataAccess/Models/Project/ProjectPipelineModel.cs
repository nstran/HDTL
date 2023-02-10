using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ProjectPipelineModel
    {
        public int Stt { get; set; }
        public string ProjectName { get; set; }
        public DateTime? NgayBatDauDuKien { get; set; }
        public DateTime? NgayKetThucDuKien { get; set; }
        public DateTime? NgayBatDauThucTe { get; set; }
        public DateTime? NgayKetThucThucTe { get; set; }
        public DateTime? NgayKyBienBanNghiemThu { get; set; }
        public decimal NgayCongTheoNganSach { get; set; }
        public decimal VndTheoNganSach { get; set; }
        public decimal UsdTheoNganSach { get; set; }
        public decimal NgayCongTheoThucTe { get; set; }
        public decimal VndTheoThucTe { get; set; }
        public decimal UsdTheoThucTe { get; set; }
        public decimal HieuQuaSuDungNguonLuc { get; set; }
        public decimal TienDo { get; set; }
        public string TrangThaiDuAn { get; set; }
        public string TrangThaiCode { get; set; }
        public string CacVanDeHienTai { get; set; }
        public string RuiRo { get; set; }
        public string BaiHoc { get; set; }
        public string GhiChu { get; set; }
        public decimal MucDoUuTienCode { get; set; } //1: Thấp; 2: Trung Bình; 3: Cao
    }
}
