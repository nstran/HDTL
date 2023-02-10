
using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class CreateOrUpdateChiTietYeuCauCapPhatParameter : BaseParameter
    {
        public YeuCauCapPhatTaiSanChiTiet YeuCauCapPhatTaiSanChiTiet { get; set; }
        public Guid UserId { get; set; }
    }
}
