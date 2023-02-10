using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Address
{
    public class DistrictEntityModel : BaseModel<DataAccess.Databases.Entities.District>
    {
        public Guid DistrictId { get; set; }
        public Guid ProvinceId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictType { get; set; }
        public bool? Active { get; set; }
        public List<WardEntityModel> WardList { get; set; }

        public DistrictEntityModel()
        {

        }

        public DistrictEntityModel(DataAccess.Databases.Entities.District entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.District ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.District();
            Mapper(this, entity);
            return entity;
        }
    }

}
