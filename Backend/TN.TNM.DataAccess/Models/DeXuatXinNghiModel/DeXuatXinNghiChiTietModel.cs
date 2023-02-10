using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.DeXuatXinNghiModel
{
    public class DeXuatXinNghiChiTietModel : BaseModel<DeXuatXinNghiChiTiet>
    {
        public int? DeXuatXinNghiChiTietId { get; set; }
        public int? DeXuatXinNghiId { get; set; }
        public DateTime Ngay { get; set; }
        public int LoaiCaLamViecId { get; set; }

        //Map từ Entity => Model
        public DeXuatXinNghiChiTietModel(DeXuatXinNghiChiTiet entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override DeXuatXinNghiChiTiet ToEntity()
        {
            var entity = new DeXuatXinNghiChiTiet();
            Mapper(this, entity);
            return entity;
        }
    }
}
