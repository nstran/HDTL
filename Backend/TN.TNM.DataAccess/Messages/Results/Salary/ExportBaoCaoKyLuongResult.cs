using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class ExportBaoCaoKyLuongResult : BaseResult
    {
        public List<string> ListHeaderBaoBao { get; set; }
        public List<ExportBaoCaoKyLuongModel> ListDataBaoCao { get; set; }
        public string ThangNamBatDauKyLuongTiengAnh { get; set; }
        public List<ExportBaoCaoKyLuong2Bang1Model> ListDataBaoCao2Bang1 { get; set; }
        public List<ExportBaoCaoKyLuong2Bang2Model> ListDataBaoCao2Bang2 { get; set; }
        public List<ExportBaoCaoKyLuong2Bang3Model> ListDataBaoCao2Bang3 { get; set; }
        public List<ExportBaoCaoKyLuong2Bang3Model> ListDataBaoCao2Bang4 { get; set; }
        public List<ExportBaoCaoKyLuong2Bang3Model> ListDataBaoCao2Bang5 { get; set; }
        public List<ExportBaoCaoKyLuong3Bang1Model> ListDataBaoCao3Bang1 { get; set; }
        public List<ExportBaoCaoKyLuong3Bang1Model> ListDataBaoCao3Bang2 { get; set; }
        public List<ExportBaoCaoKyLuong3Bang1Model> ListDataBaoCao3Bang3 { get; set; }
    }
}
