using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class TroCapChucVuMappingModel : BaseModel<TroCapChucVuMapping>
    {
        public int? TroCapChucVuMappingId { get; set; }
        public int? TroCapId { get; set; }
        public Guid PositionId { get; set; }

        public TroCapChucVuMappingModel()
        {

        }

        //Map từ Entity => Model
        public TroCapChucVuMappingModel(TroCapChucVuMapping entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override TroCapChucVuMapping ToEntity()
        {
            var entity = new TroCapChucVuMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
