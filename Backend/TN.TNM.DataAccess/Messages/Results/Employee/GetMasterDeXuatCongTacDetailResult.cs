using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDeXuatCongTacDetailResult : BaseResult
    {
        public DeXuatCongTacEntityModel DeXuatCongTac { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public bool IsShowGuiPheDuyet { get; set; }
        public bool IsShowPheDuyet { get; set; }
        public bool IsShowTuChoi { get; set; }
        public bool IsShowLuu { get; set; }
        public bool IsShowXoa { get; set; }
        public bool IsShowDatVeMoi { get; set; }
    }
}
