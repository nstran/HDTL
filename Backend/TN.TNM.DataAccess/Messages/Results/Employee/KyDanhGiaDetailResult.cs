using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class KyDanhGiaDetailResult: BaseResult
    {
        public List<PositionModel> ListChucVu { get; set; }
        public List<OrganizationEntityModel> ListPhongBan { get; set; }
        public List<PhieuDanhGia> ListPhieuDanhGia { get; set; }
        public List<NoiDungKyDanhGiaEntityModel> ListCauHinhPhieuDanhGia { get; set; }
        public List<NhanVienKyDanhGiaEntityModel> ListNhanVienKyDanhGia { get; set; }
        public KyDanhGiaEntityModel KyDanhGia { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public List<EmployeeEntityModel> ListNhanVienResult { get; set; }
        public List<MucDanhGiaDanhGiaMapping> ListMucDanhGia { get; set; }
        public List<MucDanhGia> ListThangDiemDanhGia { get; set; }
        public decimal? SoTienQuyLuongConLai { get; set; }
    }
}
