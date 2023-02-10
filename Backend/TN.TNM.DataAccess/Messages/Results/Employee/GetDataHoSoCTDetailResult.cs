using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetDataHoSoCTDetailResult : BaseResult
    {
        public HoSoCongTacEntityModel HoSoCongTac { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public List<DeNghiTamHoanUngEntityModel> ListDeNghiTamHoanUng { get; set; }
    }
}
