using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class TroCapDieuKienHuongMappingModel : BaseModel<TroCapDieuKienHuongMapping>
    {
        public int? TroCapDieuKienHuongMappingId { get; set; }
        public int? TroCapId { get; set; }
        public Guid DieuKienHuongId { get; set; }

        public TroCapDieuKienHuongMappingModel()
        {

        }

        //Map từ Entity => Model
        public TroCapDieuKienHuongMappingModel(TroCapDieuKienHuongMapping entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override TroCapDieuKienHuongMapping ToEntity()
        {
            var entity = new TroCapDieuKienHuongMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
