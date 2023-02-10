using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeXuatChucVuNhanVienEntityModel
    {
        public int? NhanVienDeXuatThayDoiChucVuId { get; set; }
        public int? DeXuatThayDoiChucVuId { get; set; }
        public Guid EmployeeId { get; set; }
        public string LyDoDeXuat { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int TrangThai { get; set; }
        public bool? Active { get; set; }

        public DateTime? DateOfBirth { get; set; }

        
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }


        public Guid? PhongBanId { get; set; }
        public string OrganizationName { get; set; }

        public Guid? ChucVuHienTaiId { get; set; }
        public string PositionName { get; set; }

        public Guid? ChucVuDeXuatId { get; set; }
        public string PositionNameDx { get; set; }

        public string LyDo { get; set; }
        public string NghiaVu { get; set; }

        public string IdentityId { get; set; } //cmnd
        public DateTime? IdentityIddateOfIssue { get; set; } //ngày cấp
        public string IdentityIdplaceOfIssue { get; set; } //nơi cấp
        public string NoiCapCmndtiengAnh { get; set; } //nơi cấp tiếng anh
        public string HoKhauThuongTruTv { get; set; }
        public string HoKhauThuongTruTa { get; set; }
        public string Address { get; set; } //Nơi ở hiện tại
        public string AddressTiengAnh { get; set; } //nơi ở hiện tại - tiếng anh

    }
}
