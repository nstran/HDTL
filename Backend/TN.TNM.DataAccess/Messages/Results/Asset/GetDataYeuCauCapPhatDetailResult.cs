
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetDataYeuCauCapPhatDetailResult : BaseResult
    {
        public YeuCauCapPhatTaiSanEntityModel YeuCauCapPhat { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public bool IsShowGuiPheDuyet{ get; set; }
        public bool IsShowPheDuyet { get; set; }
        public bool IsShowTuChoi { get; set; }
        public bool IsShowHuyYeuCauPheDuyet { get; set; }
        public bool IsShowLuu { get; set; }
        public bool IsShowXoa { get; set; }
        public bool IsShowHuy { get; set; }
        public bool IsShowPhanBo { get; set; }
        public bool IsShowDatVeMoi { get; set; }
        public bool IsShowHoanThanh { get; set; }
    }
}
