using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class DeXuatChucVuDetailResult:BaseResult
    {
        public DeXuatChucVuEntityModel DeXuatChucVu { get; set; }
        public List<DeXuatChucVuNhanVienEntityModel> NhanVienDuocDeXuats { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }
        public List<PositionModel> ListPosition { get; set; }
        public CompanyConfigEntityModel CompanyConfig { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }

        public bool IsShowGuiXacNhan { get; set; }
        public bool IsShowXacNhan { get; set; }
        public bool IsShowTuChoi { get; set; }
        public bool IsShowLuu { get; set; }
        public bool IsShowXoa { get; set; }
        public bool IsShowHuy { get; set; }

        public bool IsShowHuyYeuCauXacNhan { get; set; }
        public bool IsShowHoanThanh { get; set; }
        public bool IsShowDatVeMoi { get; set; }
        public bool IsShowNgayApDung { get; set; }

        
    }
}
