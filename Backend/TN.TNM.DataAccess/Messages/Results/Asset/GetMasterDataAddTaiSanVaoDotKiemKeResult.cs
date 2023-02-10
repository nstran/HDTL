using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Asset;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetMasterDataAddTaiSanVaoDotKiemKeResult: BaseResult
    {
        public List<DotKiemKeEntityModel> ListDotKiemKe { get; set; }
    }
}
