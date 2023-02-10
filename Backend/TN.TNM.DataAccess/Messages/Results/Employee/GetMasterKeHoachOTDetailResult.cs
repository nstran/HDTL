using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.CauHinhOtMođel;
using TN.TNM.DataAccess.Models.ChamCong;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.OT;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterKeHoachOTDetailResult: BaseResult
    {
        public List<CategoryEntityModel> ListLoaiOt { get; set; }
        public List<TrangThaiGeneral> ListLoaiCaOt { get; set; }
        public CauHinhOtCaNgayModel CauHinhOtCaNgay { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }
        public KeHoachOtEntityModel KeHoachOt { get; set; }
        public List<EmployeeEntityModel> NguoiDeXuat { get; set; }
        public List<EmployeeEntityModel> CurrentEmp { get; set; }
        public List<KeHoachOtPhongBanEntityModel> ListPhongBanOT { get; set; }
        public List<KeHoachOtThanhVienEntityModel> ListOTThanhVien { get; set; }

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

        public bool IsShowDangKyOT { get; set; }
        public bool IsShowHuyDangKyOT { get; set; }
        public bool IsShowOtKeHoachKhac { get; set; }
        public bool IsPheDuyetTemLead { get; set; }
        public Guid? UserTbpOrganizationId { get; set; }
        public bool IsNguoiTao { get; set; }
    }
}
