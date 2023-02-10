using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class MucHuongDmvsModel : BaseModel<MucHuongDmvs>
    {
        public int? MucHuongDmvsId { get; set; }
        public int? TroCapId { get; set; }
        public int HinhThucTru { get; set; }
        public decimal MucTru { get; set; }
        public decimal SoLanTu { get; set; }
        public decimal? SoLanDen { get; set; }

        /* Virtual Field */
        public string HinhThucTruText { get; set; }
        public string CachTinh { get; set; }

        public MucHuongDmvsModel()
        {

        }

        //Map từ Entity => Model
        public MucHuongDmvsModel(MucHuongDmvs entity)
        {
            var listHinhThucTru = GeneralList.GetTrangThais("TroCap_HinhThucTru");
            HinhThucTruText = listHinhThucTru.FirstOrDefault(x => x.Value == entity.HinhThucTru)?.Name;

            if (entity.SoLanDen != null && entity.SoLanTu == entity.SoLanDen) CachTinh = "Số lần đi muộn về sớm: " + ConvertNumber(entity.SoLanTu) + " lần/tháng";
            else if (entity.SoLanDen != null) CachTinh = "Số lần đi muộn về sớm: Trên " + ConvertNumber(entity.SoLanTu) + " lần đến " + ConvertNumber(entity.SoLanDen.Value) + " lần/tháng";
            else if (entity.SoLanDen == null) CachTinh = "Số lần đi muộn về sớm: Trên " + ConvertNumber(entity.SoLanTu) + " lần/tháng";

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override MucHuongDmvs ToEntity()
        {
            var entity = new MucHuongDmvs();
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
