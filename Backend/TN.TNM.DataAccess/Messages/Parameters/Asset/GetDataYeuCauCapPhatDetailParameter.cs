
using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class GetDataYeuCauCapPhatDetailParameter : BaseParameter
    {
        public int YeuCauCapPhatTaiSanId { get; set; }
        public Guid UserId { get; set; }
    }
}
