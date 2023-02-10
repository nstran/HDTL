using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class PhieuDanhGiaDetailResult: BaseResult
    {
        public List<TrangThaiGeneral> ListDangCauTraLoi { get; set; }
        public List<CategoryEntityModel> ListItemCauTraLoi { get; set; }

        public List<CauHoiPhieuDanhGiaMappingEntityModel> ListCauHoiMapping { get; set; }

        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }

        public EmployeeEntityModel NguoiTao { get; set; }
        public PhieuDanhGia PhieuDanhGia { get; set; }
        public List<MucDanhGia> ListThangDiemDanhGia { get; set; }
    }
}
