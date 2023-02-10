using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.ChamCong
{
    public class CauHinhOtCaNgayModel : BaseModel<CauHinhOtCaNgay>
    {
        public int? CauHinhOtCaNgayId { get; set; }
        public TimeSpan GioVaoSang { get; set; }
        public TimeSpan GioRaSang { get; set; }
        public TimeSpan GioKetThucSang { get; set; }
        public TimeSpan GioVaoChieu { get; set; }
        public TimeSpan GioRaChieu { get; set; }
        public TimeSpan GioKetThucChieu { get; set; }

        public CauHinhOtCaNgayModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhOtCaNgayModel(CauHinhOtCaNgay entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhOtCaNgay ToEntity()
        {
            var entity = new CauHinhOtCaNgay();
            Mapper(this, entity);
            return entity;
        }
    }
}
