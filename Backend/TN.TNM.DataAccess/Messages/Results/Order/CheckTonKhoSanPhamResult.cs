using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.KiemTraTonKho;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class CheckTonKhoSanPhamResult:BaseResult
    {
        public int KTTonKho { get; set; }
        public List<KiemTraTonKhoEntityModel> ListKiemTraTonKho { get; set; }
    }
}
