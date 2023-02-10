using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.QuyTrinh
{
    public class PhongBanTrongCacBuocQuyTrinhModel
    {
        public Guid? Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? CacBuocQuyTrinhId { get; set; }
    }
}
