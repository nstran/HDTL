using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class TaoPhanBoTaiSanResult : BaseResult
    {
        public List<int> ListAssetId { get; set; }
    }
}
