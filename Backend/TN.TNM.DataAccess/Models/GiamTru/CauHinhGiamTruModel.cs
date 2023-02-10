using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.GiamTru
{
    public class CauHinhGiamTruModel : BaseModel<CauHinhGiamTru>
    {
        public int? CauHinhGiamTruId { get; set; }
        public int LoaiGiamTruId { get; set; }
        public decimal MucGiamTru { get; set; }
        public DateTime? CreatedDate { get; set; }

        /* Virtual Field */
        public int? Stt { get; set; }
        public string TenLoaiGiamTru { get; set; }

        public CauHinhGiamTruModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhGiamTruModel(CauHinhGiamTru entity)
        {
            var listLoaiGiamTru = GeneralList.GetTrangThais("LoaiGiamTru");
            var loaiGiamTru = listLoaiGiamTru.FirstOrDefault(x => x.Value == entity.LoaiGiamTruId);
            TenLoaiGiamTru = loaiGiamTru?.Name;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhGiamTru ToEntity()
        {
            var entity = new CauHinhGiamTru();
            Mapper(this, entity);
            return entity;
        }
    }
}
