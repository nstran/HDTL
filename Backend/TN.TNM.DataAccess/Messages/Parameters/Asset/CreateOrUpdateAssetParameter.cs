
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class CreateOrUpdateAssetParameter : BaseParameter
    {
        public TaiSan TaiSan { get; set; }
        public bool IsQuick { get; set; }
    }
}
