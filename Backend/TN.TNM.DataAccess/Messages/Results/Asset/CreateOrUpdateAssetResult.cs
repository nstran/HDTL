using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Asset;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class CreateOrUpdateAssetResult : BaseResult
    {
        public int AssetId { get; set; }
        public List<AssetEntityModel> ListTaiSanChuaPhanBo { get; set; }
    }
}
