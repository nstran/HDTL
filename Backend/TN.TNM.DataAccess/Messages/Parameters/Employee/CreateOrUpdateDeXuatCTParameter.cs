
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateOrUpdateDeXuatCTParameter : BaseParameter
    {
        public DeXuatCongTac DeXuatCongTac { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
        public List<ChiTietDeXuatCongTac> ListChiTietDeXuatCongTac { get; set; }
        public List<FileUploadEntityModel> ListFileDelete { get; set; }
    }
}
