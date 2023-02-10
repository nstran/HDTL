
using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class GetAllYeuCauCapPhatTSListParameter : BaseParameter
    {
        public string MaYeuCau { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public int? TrangThai { get; set; }
    }
}
