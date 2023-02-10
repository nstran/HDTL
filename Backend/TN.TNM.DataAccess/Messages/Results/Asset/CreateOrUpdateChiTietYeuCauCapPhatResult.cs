using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Asset;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class CreateOrUpdateChiTietYeuCauCapPhatResult : BaseResult
    {
        public List<YeuCauCapPhatTaiSanChiTietEntityModel> ListTaiSanYeuCau { get; set; }
    }
}
