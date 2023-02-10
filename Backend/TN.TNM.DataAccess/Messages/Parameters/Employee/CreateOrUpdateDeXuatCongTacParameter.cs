using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateOrUpdateDeXuatCongTacParameter : BaseParameter
    {
        public DeXuatCongTac DeXuatCongTac { get; set; }
        public List<ChiTietDeXuatCongTac> ListChiTietDeXuatCongTac { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }

}
