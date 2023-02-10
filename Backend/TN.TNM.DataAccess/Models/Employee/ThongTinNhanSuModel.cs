using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ThongTinNhanSuModel
    {
        //employee
        public Guid EmployeeId { get; set; }
        public string CodeMayChamCong { get; set; }
        public string TenMayChamCong { get; set; }
        public int? SubCode1Value { get; set; }
        public int? SubCode2Value { get; set; }
        public int? DeptCodeValue { get; set; }
        public Guid? CapBacId { get; set; }
        public string MaTest { get; set; }
        public string DiemTest { get; set; }

        //contact
        public string IdentityId { get; set; } //cmnd
        //public DateTime? IdentityIddateOfParticipation { get; set; }
        public DateTime? IdentityIddateOfIssue { get; set; } //ngày cấp
        public string IdentityIdplaceOfIssue { get; set; } //nơi cấp
        public string NoiCapCmndtiengAnh { get; set; } //nơi cấp tiếng anh
        public string NguyenQuan { get; set; }
        public string NoiSinh { get; set; }
        public string HoKhauThuongTruTv { get; set; }
        public string HoKhauThuongTruTa { get; set; }
        public string Address { get; set; } //Nơi ở hiện tại
        public string AddressTiengAnh { get; set; } //nơi ở hiện tại - tiếng anh
        public Guid? ProvinceId { get; set; }
    }
}
