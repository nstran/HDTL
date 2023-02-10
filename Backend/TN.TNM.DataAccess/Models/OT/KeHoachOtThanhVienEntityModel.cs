using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.OT
{
    public class KeHoachOtThanhVienEntityModel
    {
        public int? ThanVienOtId { get; set; }
        public int? KeHoachOtId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public Guid? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public Guid? PositionId { get; set; }
        public string PositionName { get; set; }
        public short? TrangThai { get; set; }
        public string GhiChu { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
