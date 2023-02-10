using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CapNhatKyDanhGiaParameter: BaseParameter
    {
        public KyDanhGiaEntityModel KyDanhGia { get; set; }
        public List<NhanVienKyDanhGiaEntityModel> NhanVienKyDanhGia { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }
}
