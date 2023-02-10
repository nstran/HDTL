using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class TroCapLoaiHopDongMappingModel : BaseModel<TroCapLoaiHopDongMapping>
    {
        public int? TroCapLoaiHopDongMappingId { get; set; }
        public int? TroCapId { get; set; }
        public Guid LoaiHopDongId { get; set; }

        public TroCapLoaiHopDongMappingModel()
        {

        }

        //Map từ Entity => Model
        public TroCapLoaiHopDongMappingModel(TroCapLoaiHopDongMapping entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override TroCapLoaiHopDongMapping ToEntity()
        {
            var entity = new TroCapLoaiHopDongMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
