using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeXuatCongTacChiTietEntityModel
    {
        public int ChiTietDeXuatCongTacId { get; set; }
        public int DeXuatCongTacId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? PositionId { get; set; }
        public string PositionName { get; set; }
        public Guid? OrganizationId { get; set; }
        public string LyDo { get; set; }    
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string MaNV { get; set; }
        public string TenNhanVien { get; set; }
        public string PhongBan { get; set; }
        public string ViTriLamViec { get; set; }
        public string Identity { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? IdentityIddateOfIssue { get; set; } //ngày cấp
        public string IdentityIdplaceOfIssue { get; set; } //nơi cấp
        public string NoiCapCmndtiengAnh { get; set; } //nơi cấp tiếng anh
    
    }
}
