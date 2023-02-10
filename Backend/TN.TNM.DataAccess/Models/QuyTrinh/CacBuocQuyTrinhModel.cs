using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.QuyTrinh
{
    public class CacBuocQuyTrinhModel
    {
        public Guid? Id { get; set; }
        public int Stt { get; set; }
        public int LoaiPheDuyet { get; set; }
        public Guid? CauHinhQuyTrinhId { get; set; }
        public List<PhongBanTrongCacBuocQuyTrinhModel> ListPhongBanTrongCacBuocQuyTrinh { get; set; }
    }
}
