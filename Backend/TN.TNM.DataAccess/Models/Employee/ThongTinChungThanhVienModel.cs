using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ThongTinChungThanhVienModel
    {
        // Employee
        public Guid? EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string AvatarUrl { get; set; }
        public string UserName { get; set; }
        public int TrangThaiId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public DateTime? StartDateMayChamCong { get; set; }
        public string QuocTich { get; set; }
        public string DanToc { get; set; }
        public string TonGiao { get; set; }
        public Guid? PositionId { get; set; }
        public bool IsOverviewer { get; set; }
        public bool IsNhanSu { get; set; }
        public DateTime? NgayNghiViec { get; set; }

        // Contact
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; } //SDT cá nhân
        public string WorkPhone { get; set; } // SDT cty
        public string OtherPhone { get; set; } //SDT nhà riêng
        public string Email { get; set; } //Email cá nhân
        public string WorkEmail { get; set; } //Email cty
        public string FirstName { get; set; } //Họ + tên đệm TV
        public string LastName { get; set; } //Tên TV
        public string TenTiengAnh { get; set; } //Full tên TA
        public string GioiTinh { get; set; }
        public string ViTriLamViec { get; set; }
        public int? Benefit { get; set; }
        public decimal? PercenBenefit { get; set; }
        public int? EmployeeType { get; set; }

        // Thông tin nghỉ phép
        public decimal? SoNgayDaNghiPhep { get; set; }
        public decimal? SoNgayPhepConLai { get; set; }
        public Guid? Mission { get; set; }
    }
}
