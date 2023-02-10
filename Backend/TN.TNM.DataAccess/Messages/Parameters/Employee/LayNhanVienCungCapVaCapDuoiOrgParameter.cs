using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class LayNhanVienCungCapVaCapDuoiOrgParameter: BaseParameter
    {
        public Guid OrgId { get; set; }
        public int TrangThai { get; set; }
        public int KyDanhGiaId { get; set; }
        public bool IsShowTaoDeXuatTangLuong { get; set; }
    }
}
