using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.QuyTrinh
{
    public class CauHinhQuyTrinhModel
    {
        public Guid? Id { get; set; }
        public decimal SoTienTu { get; set; }
        public string TenCauHinh { get; set; }
        public string QuyTrinh { get; set; }
        public Guid? QuyTrinhId { get; set; }
        public List<CacBuocQuyTrinhModel> ListCacBuocQuyTrinh { get; set; }
    }
}
