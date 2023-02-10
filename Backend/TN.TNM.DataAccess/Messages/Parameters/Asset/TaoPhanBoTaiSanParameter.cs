
using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class TaoPhanBoTaiSanParameter : BaseParameter
    {
        public List<CapPhatTaiSan> ListPhanBo { get; set; }
        public Guid UserId { get; set; }
    }
}
