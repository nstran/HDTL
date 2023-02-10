using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CauHinhNghiLe
{
    public class CauHinhNghiLeModel : BaseModel<Databases.Entities.CauHinhNghiLe>
    {
        public int? NghiLeId { get; set; }
        public int SoNam { get; set; }

        /* Virtual Field */
        public List<CauHinhNghiLeChiTietModel> ListCauHinhNghiLeChiTiet { get; set; }
        public string NgayNghiLe { get; set; }
        public string NgayNghiBu { get; set; }
        public string NgayLamBu { get; set; }

        public CauHinhNghiLeModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhNghiLeModel(Databases.Entities.CauHinhNghiLe entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override Databases.Entities.CauHinhNghiLe ToEntity()
        {
            var entity = new Databases.Entities.CauHinhNghiLe();
            Mapper(this, entity);
            return entity;
        }
    }
}
