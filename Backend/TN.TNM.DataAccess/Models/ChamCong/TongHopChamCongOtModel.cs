using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.ChamCong
{
    public class TongHopChamCongOtModel : BaseModel<TongHopChamCongOt>
    {
        public int? TongHopChamCongOtId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public Guid LoaiOtId { get; set; }
        public decimal SoPhut { get; set; }
        public decimal SoGio { get; set; }

        /* Virtual Field */
        public string LoaiOtName { get; set; }

        public TongHopChamCongOtModel()
        {

        }

        //Map từ Entity => Model
        public TongHopChamCongOtModel(TongHopChamCongOt entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override TongHopChamCongOt ToEntity()
        {
            var entity = new TongHopChamCongOt();
            Mapper(this, entity);
            return entity;
        }
    }
}
