
using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class GetAllAssetListParameter : BaseParameter
    {
        public string MaTaiSan { get; set; }
        public string TenTaiSan { get; set; }
        public List<int?> ListTrangThai { get; set; }
        public string MaYeuCau { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public List<Guid> ListLoaiTS { get; set; }
        public DateTime? NgayTinhKhauHao { get; set; }
        public List<Guid> ListProvinceId { get; set; }
    }
}
