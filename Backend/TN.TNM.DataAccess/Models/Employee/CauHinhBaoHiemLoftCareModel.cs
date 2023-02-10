using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class CauHinhBaoHiemLoftCareModel : BaseModel<CauHinhBaoHiemLoftCare>
    {
        public int? CauHinhBaoHiemLoftCareId { get; set; }
        public string NamCauHinh { get; set; }

        // Virtual
        public NhomBaoHiemLoftCareModel NhomBaoHiemLoftCare { get; set; }
        public QuyenLoiBaoHiemLoftCareModel QuyenLoiBaoHiemLoftCare { get; set; }
        public List<NhomBaoHiemLoftCareModel> ListNhomBaoHiemLoftCare { get; set; }
        public List<QuyenLoiBaoHiemLoftCareModel> ListQuyenLoiBaoHiemLoftCare { get; set; }

        public CauHinhBaoHiemLoftCareModel()
        {
            ListNhomBaoHiemLoftCare = new List<NhomBaoHiemLoftCareModel>();
            ListQuyenLoiBaoHiemLoftCare = new List<QuyenLoiBaoHiemLoftCareModel>();
        }

        //Map từ Entity => Model
        public CauHinhBaoHiemLoftCareModel(CauHinhBaoHiemLoftCare entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhBaoHiemLoftCare ToEntity()
        {
            var entity = new CauHinhBaoHiemLoftCare();
            Mapper(this, entity);
            return entity;
        }
    }
}
