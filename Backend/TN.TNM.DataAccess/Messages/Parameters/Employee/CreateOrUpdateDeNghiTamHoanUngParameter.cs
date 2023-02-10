
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateOrUpdateDeNghiTamHoanUngParameter : BaseParameter
    {
        public DeNghiTamHoanUng DeNghiTamHoanUng { get; set; }
        public List<DeNghiTamHoanUngChiTiet> ListNoiDungTT { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }

    }
}
