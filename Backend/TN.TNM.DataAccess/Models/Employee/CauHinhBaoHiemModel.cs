using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class CauHinhBaoHiemModel : BaseModel<CauHinhBaoHiem>
    {
        public int? CauHinhBaoHiemId { get; set; }
        public int? LoaiDong { get; set; }
        public decimal MucDong { get; set; }
        public decimal MucDongToiDa { get; set; }
        public decimal MucLuongCoSo { get; set; }
        public decimal TiLePhanBoMucDongBhxhcuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhxhcuaNsdld { get; set; }
        public decimal TiLePhanBoMucDongBhytcuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhytcuaNsdld { get; set; }
        public decimal TiLePhanBoMucDongBhtncuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhtncuaNsdld { get; set; }
        public decimal TiLePhanBoMucDongBhtnnncuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhtnnncuaNsdld { get; set; }

        public CauHinhBaoHiemModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhBaoHiemModel(CauHinhBaoHiem entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhBaoHiem ToEntity()
        {
            var entity = new CauHinhBaoHiem();
            Mapper(this, entity);
            return entity;
        }
    }
}
