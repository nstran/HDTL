using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.QuyTrinh
{
    public class QuyTrinhModel
    {
        public Guid? Id { get; set; }
        public string TenQuyTrinh { get; set; }
        public string MaQuyTrinh { get; set; }
        public int DoiTuongApDung { get; set; }
        public bool HoatDong { get; set; }
        public string MoTa { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public string NguoiTao { get; set; }
        public string NgayTao { get; set; }
        public string TenDoiTuongApDung { get; set; }
    }
}
