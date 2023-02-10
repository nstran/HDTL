using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class ImportAssetResult:BaseResult
    {
        public List<TaiSan> ListFail { get; set; }
    }
}
