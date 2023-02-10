using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Asset;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class DotKiemKeSearchResult: BaseResult
    {
        public List<DotKiemKeEntityModel> ListDotKiemKe { get; set; }
        public List<TrangThaiGeneral> ListTrangThaiKiemKe { get; set; }
    }
}
