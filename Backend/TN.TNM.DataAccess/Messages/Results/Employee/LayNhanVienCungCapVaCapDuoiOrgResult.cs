using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class LayNhanVienCungCapVaCapDuoiOrgResult: BaseResult
    {
        public List<EmployeeEntityModel> ListEmp { get; set; }
        public List<NhanVienKyDanhGiaEntityModel> ListNhanVienKyDanhGia { get; set; }
        public bool IsTruongPhong { get; set; }
        public decimal? TongMucTangQuanLy { get; set; }
        public decimal? QuyLuongConLai { get; set; }
        public Guid? EmpIdTruongPhong { get; set; }
        public bool IsShowQuyLuongVaMucTang { get; set; }

    }
}
