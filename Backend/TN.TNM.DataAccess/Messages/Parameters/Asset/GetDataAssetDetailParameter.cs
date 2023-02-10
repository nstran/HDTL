
using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class GetDataAssetDetailParameter : BaseParameter
    {
        public int TaiSanId { get; set; }
        public Guid UserId { get; set; }
    }
}
