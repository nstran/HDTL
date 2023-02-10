
using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class BaoCaoKhauHaoParameter : BaseParameter
    {

        public DateTime? ThoiGianKhauHaoDen { get; set; }     
        public List<Guid> ListPhanLoaiTaiSanId { get; set; }
        public List<int> ListHienTrangTaiSan { get; set; }

    }
}
