using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class CreateOrUpdateChiTietDeXuatCongTacResult : BaseResult
    {
        public List<DeXuatCongTacChiTietEntityModel> ListDeXuatCongTacChiTietTemp { get; set; }
    }
}
