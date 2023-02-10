using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.ChamCong
{
    public class ChamCongOtModel : BaseModel<ChamCongOt>
    {
        public int? ChamCongOtId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LoaiOtId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public TimeSpan? GioVaoSang { get; set; }
        public TimeSpan? GioRaSang { get; set; }
        public TimeSpan? GioVaoChieu { get; set; }
        public TimeSpan? GioRaChieu { get; set; }
        public TimeSpan? GioVaoToi { get; set; }
        public TimeSpan? GioRaToi { get; set; }

        public ChamCongOtModel()
        {

        }

        //Map từ Entity => Model
        public ChamCongOtModel(ChamCongOt entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override ChamCongOt ToEntity()
        {
            var entity = new ChamCongOt();
            Mapper(this, entity);
            return entity;
        }
    }
}
