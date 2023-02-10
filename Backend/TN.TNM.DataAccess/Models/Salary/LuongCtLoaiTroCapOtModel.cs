using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtLoaiTroCapOtModel : BaseModel<LuongCtLoaiTroCapOt>
    {
        public int LuongCtLoaiTroCapOtId { get; set; }
        public int LuongCtTroCapOtId { get; set; }
        public int KyLuongId { get; set; }
        public Guid LoaiOtId { get; set; }
        public decimal MucTroCap { get; set; }
        public decimal SoGioOt { get; set; }

        public LuongCtLoaiTroCapOtModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtLoaiTroCapOtModel(LuongCtLoaiTroCapOt entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtLoaiTroCapOt ToEntity()
        {
            var entity = new LuongCtLoaiTroCapOt();
            Mapper(this, entity);
            return entity;
        }
    }
}
