using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class QuyenLoiBaoHiemLoftCareModel : BaseModel<QuyenLoiBaoHiemLoftCare>
    {
        public int? QuyenLoiBaoHiemLoftCareId { get; set; }
        public int? NhomBaoHiemLoftCareId { get; set; }
        public string TenQuyenLoi { get; set; }

        public List<MucHuongBaoHiemLoftCareModel> ListMucHuongBaoHiemLoftCare { get; set; }

        public QuyenLoiBaoHiemLoftCareModel()
        {
            ListMucHuongBaoHiemLoftCare = new List<MucHuongBaoHiemLoftCareModel>();
        }

        //Map từ Entity => Model
        public QuyenLoiBaoHiemLoftCareModel(QuyenLoiBaoHiemLoftCare entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override QuyenLoiBaoHiemLoftCare ToEntity()
        {
            var entity = new QuyenLoiBaoHiemLoftCare();
            Mapper(this, entity);
            return entity;
        }
    }
}
