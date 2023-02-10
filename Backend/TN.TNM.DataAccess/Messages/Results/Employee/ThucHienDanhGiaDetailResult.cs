using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class ThucHienDanhGiaDetailResult: BaseResult
    {
        public MucDanhGia ThangDiemDanhGia { get; set; }
        public DanhGiaNhanVienEntityModel DanhGiaNhanVien { get; set; }
        public List<TrangThaiGeneral> ListDangCauTraLoi { get; set; }
        public List<CategoryEntityModel> ListItemCauTraLoi { get; set; }
        public List<ChiTietDanhGiaNhanVienEntityModel> ListCauTraLoiMapping { get; set; }
        public List<MucDanhGiaDanhGiaMappingEntityModel> ListMucDanhGia { get; set; }
        public int? CachTinhDiem { get; set; }
        public bool? IsNhanVienTuDanhGia { get; set; }
        public bool? IsQuanLyDanhGia { get; set; }
        public bool? IsTruongPhongDanhGia { get; set; }
    }
}
