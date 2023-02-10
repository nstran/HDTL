using System;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class BaoCaoPhanBoEntityModel
    {
        public int TaiSanId { get; set; }
        public string MaTaiSan { get; set; }
        public string TenTaiSan { get; set; }       
        public Guid? PhanLoaiTaiSanId { get; set; }     
        public int HienTrangTaiSan { get; set; }
        public string HienTrangTaiSanString { get; set; }
        public DateTime? NgayVaoSo { get; set; }
        public Guid? EmployeeId { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public Guid? OrganizationId { get; set; }
        public string PhongBan { get; set; }
        public Guid PositionId { get; set; }     
        public string MoTa { get; set; }
        public string ViTriLamViec { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string PhanLoaiTaiSan { get; set; }
        public Guid? NguoiSuDungId { get; set; }
    }
}
