
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class DownloadTemplateAssetParameter : BaseParameter
    {
        public int PhanLoai { get; set; } // 1: Cấp phát   0: Thu hồi
    }
}
