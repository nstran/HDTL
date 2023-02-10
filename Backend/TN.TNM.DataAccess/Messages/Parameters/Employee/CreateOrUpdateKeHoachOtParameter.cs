using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateOrUpdateKeHoachOtParameter : BaseParameter
    {
        public KeHoachOtEntityModel KeHoachOt { get; set; }
        public List<KeHoachOtPhongBanEntityModel> ListKeHoachOtPhongBan { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }

}
