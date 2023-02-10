using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetDataDeNghiDetailFormResult : BaseResult
    {
        public List<HoSoCongTacEntityModel> ListHoSoCongTac { get; set; }
        public List<CategoryEntityModel> ListHinhThucTT { get; set; }
        public DeNghiTamHoanUngEntityModel DeNghiTamHoanUng { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public List<DeNghiTamHoanUngChiTiet> ListNoiDungTT { get; set; }
        public bool IsShowGuiPheDuyet { get; set; }
        public bool IsShowPheDuyet { get; set; }
        public bool IsShowTuChoi { get; set; }
        public bool IsShowLuu { get; set; }
        public bool IsShowXoa { get; set; }
        public bool IsShowDatVeMoi { get; set; }
    }
}
