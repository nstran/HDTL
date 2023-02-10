using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class TrinhDoHocVanTuyenDungModel
    {
        public Guid? EmployeeId { get; set; }
        public string TenTruongHocCaoNhat { get; set; }
        public string ChuyenNganhHoc { get; set; }
        public int? KyNangTayNghes { get; set; }
        public Guid? BangCapCaoNhatDatDuocId { get; set; }
        public string TomTatHocVan { get; set; }
        public Guid? PhuongThucTuyenDungId { get; set; }
        public string LoaiChuyenNganhHoc { get; set; }
        public Guid? NguonTuyenDungId { get; set; }
        public decimal? MucPhi { get; set; }
        public string KinhNghiemLamViec { get; set; }
        public List<CategoryEntityModel> ListBangCap { get; set; }
        public List<CategoryEntityModel> ListPhuongThucTuyenDung { get; set; }
        public List<CategoryEntityModel> ListNguonTuyenDung { get; set; }
        public List<BaseType> ListKyNangTayNghe { get; set; }
    }
}
