using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.QuyTrinh
{
    public class LichSuPheDuyetModel
    {
        public Guid Id { get; set; }
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string LyDo { get; set; }
        public int TrangThai { get; set; }
        public string NgayTaoString { get; set; }
        public string NguoiPheDuyet { get; set; }
        public string TenDonVi { get; set; }
        public string TenTrangThai { get; set; }
    }
}
