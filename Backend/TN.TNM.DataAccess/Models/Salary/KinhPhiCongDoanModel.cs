using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class KinhPhiCongDoanModel : BaseModel<KinhPhiCongDoan>
    {
        public int? KinhPhiCongDoanId { get; set; }
        public decimal MucDongNhanVien { get; set; }
        public decimal MucDongCongTy { get; set; }

        public KinhPhiCongDoanModel()
        {

        }

        //Map từ Entity => Model
        public KinhPhiCongDoanModel(KinhPhiCongDoan entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override KinhPhiCongDoan ToEntity()
        {
            var entity = new KinhPhiCongDoan();
            Mapper(this, entity);
            return entity;
        }
    }
}
