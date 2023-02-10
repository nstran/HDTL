using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CauHinhNghiLe
{
    public class CauHinhNghiLeChiTietModel : BaseModel<CauHinhNghiLeChiTiet>
    {
        public int? NghiLeChiTietId { get; set; }
        public int? NghiLeId { get; set; }
        public int? LoaiNghiLe { get; set; }
        public DateTime Ngay { get; set; }

        public CauHinhNghiLeChiTietModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhNghiLeChiTietModel(CauHinhNghiLeChiTiet entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhNghiLeChiTiet ToEntity()
        {
            var entity = new CauHinhNghiLeChiTiet();
            Mapper(this, entity);
            return entity;
        }
    }
}
