
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class CapNhapTrangThaiYeuCauCapPhatParameter : BaseParameter
    {
        public int YeuCauCapPhatTaiSanId { get; set; }
        public int Type { get; set; }
    }
}
