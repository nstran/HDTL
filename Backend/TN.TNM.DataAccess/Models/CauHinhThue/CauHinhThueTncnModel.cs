using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CauHinhThue
{
    public class CauHinhThueTncnModel : BaseModel<CauHinhThueTncn>
    {
        public int? CauHinhThueTncnId { get; set; }
        public decimal SoTienTu { get; set; }
        public decimal SoTienDen { get; set; }
        public decimal PhanTramThue { get; set; }
        public decimal SoBiTruTheoCongThuc { get; set; }

        public CauHinhThueTncnModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhThueTncnModel(CauHinhThueTncn entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhThueTncn ToEntity()
        {
            var entity = new CauHinhThueTncn();
            Mapper(this, entity);
            return entity;
        }
    }
}
