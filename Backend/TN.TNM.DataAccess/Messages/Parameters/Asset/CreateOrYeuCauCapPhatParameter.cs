
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class CreateOrYeuCauCapPhatParameter : BaseParameter
    {
        public YeuCauCapPhatTaiSan YeuCauCapPhatTaiSan { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
        public List<YeuCauCapPhatTaiSanChiTiet> ListYeuCauCapPhatTaiSanChiTiet { get; set; }

        public List<FileUploadEntityModel> ListFileDelete { get; set; }
    }
}
