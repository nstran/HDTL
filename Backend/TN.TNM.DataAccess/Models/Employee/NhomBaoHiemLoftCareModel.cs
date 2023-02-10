using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class NhomBaoHiemLoftCareModel : BaseModel<NhomBaoHiemLoftCare>
    {
        public int? NhomBaoHiemLoftCareId { get; set; }
        public int? CauHinhBaoHiemLoftCareId { get; set; }
        public string TenNhom { get; set; }

        public List<ChucVuBaoHiemLoftCareModel> ListChucVuBaoHiemLoftCare { get; set; }

        public NhomBaoHiemLoftCareModel()
        {
            ListChucVuBaoHiemLoftCare = new List<ChucVuBaoHiemLoftCareModel>();
        }

        //Map từ Entity => Model
        public NhomBaoHiemLoftCareModel(NhomBaoHiemLoftCare entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override NhomBaoHiemLoftCare ToEntity()
        {
            var entity = new NhomBaoHiemLoftCare();
            Mapper(this, entity);
            return entity;
        }
    }
}
