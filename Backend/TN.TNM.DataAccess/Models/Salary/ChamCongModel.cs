using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class ChamCongModel : BaseModel<Databases.Entities.ChamCong>
    {
        public int? ChamCongId { get; set; }
        public Guid? EmployeeId { get; set; }
        public TimeSpan? VaoSang { get; set; }
        public TimeSpan? RaSang { get; set; }
        public TimeSpan? VaoChieu { get; set; }
        public TimeSpan? RaChieu { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime NgayChamCong { get; set; }

        public ChamCongModel()
        {

        }

        //Map từ Entity => Model
        public ChamCongModel(Databases.Entities.ChamCong entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override Databases.Entities.ChamCong ToEntity()
        {
            var entity = new Databases.Entities.ChamCong();
            Mapper(this, entity);
            return entity;
        }
    }
}
