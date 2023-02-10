using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CapNhatPhieuDanhGiaParameter: BaseParameter
    {
        public List<CauHoiPhieuDanhGiaMappingApi> CauHoiNV { get; set; }
        public List<CauHoiPhieuDanhGiaMappingApi> CauHoiQL { get; set; }
        public PhieuDanhGiaEntityModel PhieuDanhGia { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }

}
