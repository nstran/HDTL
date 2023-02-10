using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using System.Linq;

namespace TN.TNM.DataAccess.Models.CauHinhOtMođel
{
    public class CauHinhOtModel : BaseModel<CauHinhOt>
    {
        public int? CauHinhOtId { get; set; }
        public decimal TyLeHuong { get; set; }
        public Guid LoaiOtId { get; set; }
        public DateTime? CreatedDate { get; set; }

        /* Virtual Field */
        public int? Stt { get; set; }
        public string TenLoaiOt { get; set; }
        public string LoaiOtName { get; set; }

        public CauHinhOtModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhOtModel(CauHinhOt entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhOt ToEntity()
        {
            var entity = new CauHinhOt();
            Mapper(this, entity);
            return entity;
        }
    }
}
