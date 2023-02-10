using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.ChamCong
{
    public class CauHinhChamCongOtModel : BaseModel<CauHinhChamCongOt>
    {
        public int? CauHinhChamCongOtId { get; set; }
        public decimal SoPhut { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }

        public CauHinhChamCongOtModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhChamCongOtModel(CauHinhChamCongOt entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhChamCongOt ToEntity()
        {
            var entity = new CauHinhChamCongOt();
            Mapper(this, entity);
            return entity;
        }
    }
}
