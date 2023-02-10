using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class ImportAssetParameter: BaseParameter
    {
        public List<TaiSan> ListTaiSan { get; set; }
    }
}
