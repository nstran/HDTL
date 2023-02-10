using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class DotKiemKeDetailParameter: BaseParameter
    {
        public int DotKiemKeId { get; set; }
        public List<Guid> ProvincecId { get; set; }
        public List<Guid> PhanLoaiTaiSanId { get; set; }
        public int? HienTrangTaiSan { get; set; }
        public DateTime? NgayKiemKe { get; set; }
        public List<Guid> NguoiKiemKeId { get; set; }
    }
}
