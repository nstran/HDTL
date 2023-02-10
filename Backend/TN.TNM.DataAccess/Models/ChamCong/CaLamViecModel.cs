using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.ChamCong
{
    public class CaLamViecModel : BaseModel<CaLamViec>
    {
        public int? CaLamViecId { get; set; }
        public TimeSpan GioVao { get; set; }
        public TimeSpan GioRa { get; set; }
        public int LoaiCaLamViecId { get; set; }
        public TimeSpan ThoiGianKetThucCa { get; set; }

        /* Virtual Field */
        public List<CaLamViecChiTietModel> ListCaLamViecChiTiet { get; set; }
        public string TenLoaiCaLamViec { get; set; }
        public string NgayLamViecTrongTuanText { get; set; }

        public CaLamViecModel()
        {

        }

        //Map từ Entity => Model
        public CaLamViecModel(CaLamViec entity)
        {
            var listLoaiCaLamViec = GeneralList.GetTrangThais("LoaiCaLamViec").ToList();
            var LoaiCaLamViec = listLoaiCaLamViec.FirstOrDefault(x => x.Value == entity.LoaiCaLamViecId);
            TenLoaiCaLamViec = LoaiCaLamViec?.Name;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CaLamViec ToEntity()
        {
            var entity = new CaLamViec();
            Mapper(this, entity);
            return entity;
        }
    }
}
