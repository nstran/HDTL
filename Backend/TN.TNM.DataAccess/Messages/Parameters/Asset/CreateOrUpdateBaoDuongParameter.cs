
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class CreateOrUpdateBaoDuongParameter : BaseParameter
    {
        public BaoDuongTaiSan BaoDuong { get; set; }
    }
}
