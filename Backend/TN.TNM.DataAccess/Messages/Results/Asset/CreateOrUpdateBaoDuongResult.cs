using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Asset;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class CreateOrUpdateBaoDuongResult : BaseResult
    {
        public int BaoDuongId { get; set; }
        public List<BaoDuongEntityModel> ListBaoDuong { get; set; }
    }
}
