using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ChucVuBaoHiemLoftCareModel : BaseModel<ChucVuBaoHiemLoftCare>
    {
        public int? ChucVuBaoHiemLoftCareId { get; set; }
        public int? NhomBaoHiemLoftCareId { get; set; }
        public Guid PositionId { get; set; }
        public decimal? SoNamKinhNghiem { get; set; }

        public ChucVuBaoHiemLoftCareModel()
        {

        }

        //Map từ Entity => Model
        public ChucVuBaoHiemLoftCareModel(ChucVuBaoHiemLoftCare entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override ChucVuBaoHiemLoftCare ToEntity()
        {
            var entity = new ChucVuBaoHiemLoftCare();
            Mapper(this, entity);
            return entity;
        }
    }
}
