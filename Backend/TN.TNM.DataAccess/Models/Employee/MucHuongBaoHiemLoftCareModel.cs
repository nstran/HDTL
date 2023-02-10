using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using System.Linq;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class MucHuongBaoHiemLoftCareModel : BaseModel<MucHuongBaoHiemLoftCare>
    {
        public int? Id { get; set; }
        public int? QuyenLoiBaoHiemLoftCareId { get; set; }
        public int DoiTuongHuong { get; set; }
        public decimal MucHuong { get; set; }
        public int? DonVi { get; set; }
        public decimal LePhi { get; set; }
        public decimal PhiCoDinh { get; set; }
        public decimal PhiTheoLuong { get; set; }
        public decimal MucGiam { get; set; }

        //Virtual
        public string TenDoiTuong { get; set; }

        public MucHuongBaoHiemLoftCareModel()
        {

        }

        //Map từ Entity => Model
        public MucHuongBaoHiemLoftCareModel(MucHuongBaoHiemLoftCare entity)
        {
            var listDoiTuong = GeneralList.GetTrangThais("DoiTuongBaoHiemLoft");
            var doiTuong = listDoiTuong.FirstOrDefault(x => x.Value == entity.DoiTuongHuong);
            TenDoiTuong = doiTuong?.Name;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override MucHuongBaoHiemLoftCare ToEntity()
        {
            var entity = new MucHuongBaoHiemLoftCare();
            Mapper(this, entity);
            return entity;
        }
    }
}
