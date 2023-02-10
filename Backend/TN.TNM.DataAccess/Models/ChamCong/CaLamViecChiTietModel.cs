using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.ChamCong
{
    public class CaLamViecChiTietModel : BaseModel<CaLamViecChiTiet>
    {
        public int? CaLamViecChiTietId { get; set; }
        public int? CaLamViecId { get; set; }
        public int? NgayTrongTuan { get; set; }

        /* Virtual Field */
        public string NgayTrongTuanText { get; set; }

        public CaLamViecChiTietModel()
        {

        }

        //Map từ Entity => Model
        public CaLamViecChiTietModel(CaLamViecChiTiet entity)
        {
            var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
            var _ngayTrongTuan = listNgayLamViecTrongTuan.FirstOrDefault(x => x.Value == entity.NgayTrongTuan);
            NgayTrongTuanText = _ngayTrongTuan?.Name;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CaLamViecChiTiet ToEntity()
        {
            var entity = new CaLamViecChiTiet();
            Mapper(this, entity);
            return entity;
        }
    }
}
