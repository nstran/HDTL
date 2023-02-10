using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class MucHuongTheoNgayNghiModel : BaseModel<MucHuongTheoNgayNghi>
    {
        public int? MucHuongTheoNgayNghiId { get; set; }
        public int? TroCapId { get; set; }
        public decimal MucHuongPhanTram { get; set; }
        public int? LoaiNgayNghi { get; set; }
        public decimal SoNgayTu { get; set; }
        public decimal? SoNgayDen { get; set; }

        /* Virtual Field */
        public string CachTinh { get; set; }

        public MucHuongTheoNgayNghiModel()
        {

        }

        //Map từ Entity => Model
        public MucHuongTheoNgayNghiModel(MucHuongTheoNgayNghi entity)
        {
            var listLoaiNgayNghi = GeneralList.GetTrangThais("TroCap_LoaiNgayNghi");
            var loaiNgayNghi = listLoaiNgayNghi.FirstOrDefault(x => x.Value == entity.LoaiNgayNghi);
            var text = loaiNgayNghi == null ? "" : loaiNgayNghi.Name;

            if (entity.SoNgayDen != null && entity.SoNgayTu == entity.SoNgayDen) CachTinh = "Ngày nghỉ: " + ConvertNumber(entity.SoNgayTu) + " ngày " + text;
            else if (entity.SoNgayDen == null) CachTinh = "Ngày nghỉ: Trên " + ConvertNumber(entity.SoNgayTu) + " ngày " + text;
            else if (entity.SoNgayDen != null) CachTinh = "Ngày nghỉ: Trên " + ConvertNumber(entity.SoNgayTu) + " ngày đến " + ConvertNumber(entity.SoNgayDen.Value) + " ngày " + text;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override MucHuongTheoNgayNghi ToEntity()
        {
            var entity = new MucHuongTheoNgayNghi();
            Mapper(this, entity);
            return entity;
        }

        private string ConvertNumber(decimal number)
        {
            return string.Format("{0:G29}",
                decimal.Parse(number.ToString(), System.Globalization.CultureInfo.GetCultureInfo("en-US")));
        }
    }
}
