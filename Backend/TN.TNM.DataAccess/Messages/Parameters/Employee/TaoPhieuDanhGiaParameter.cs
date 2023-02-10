using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TaoPhieuDanhGiaParameter: BaseParameter
    {
        public List<CauHoiPhieuDanhGiaMappingApi> CauHoiNV { get; set; }
        public List<CauHoiPhieuDanhGiaMappingApi> CauHoiQL { get; set; }
        public PhieuDanhGiaEntityModel PhieuDanhGia { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }

    public class CauHoiPhieuDanhGiaMappingApi // lấy list dsach câu hỏi từ front -end về
    {
        public int? CauHoiPhieuDanhGiaMappingId { get; set; }
        public int? PhieuDanhGiaId { get; set; }
        public string NoiDungCauHoi { get; set; }
        public decimal? TiLe { get; set; }
        public List<CategoryEntityModel> DanhSachItem { get; set; }
        public int? ParentId { get; set; }
        public int? NguoiDanhGia { get; set; }
        public bool? IsFarther { get; set; }
        public decimal? Stt { get; set; }
        public TrangThaiGeneral CauTraLoi { get; set; }
    }

}
