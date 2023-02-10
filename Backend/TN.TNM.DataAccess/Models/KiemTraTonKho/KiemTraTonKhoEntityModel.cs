using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.KiemTraTonKho
{
    public class KiemTraTonKhoEntityModel
    {
        public int Stt { get; set; }
        public string WarehouseName { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal SoLuongDat { get; set; }
        public decimal SoLuongTonKho { get; set; }
        public decimal? SoLuongTonKhoToiDa { get; set; }
        public decimal? SoLuongTonKhoToiThieu { get; set; }
    }
}
