using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class TaiLieuNhanVienEntityModel : BaseModel<TaiLieuNhanVien>
    {
        public int? TaiLieuNhanVienId { get; set; }
        public string TenTaiLieu { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? NgayNop { get; set; }
        public DateTime? NgayHen { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CauHinhChecklistId { get; set; }

        public TaiLieuNhanVienEntityModel()
        {

        }

        //Map từ Entity => Model
        public TaiLieuNhanVienEntityModel(TaiLieuNhanVien entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override TaiLieuNhanVien ToEntity()
        {
            var entity = new TaiLieuNhanVien();
            Mapper(this, entity);
            return entity;
        }
    }
}
