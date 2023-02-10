using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtGiamTruSauThueModel : BaseModel<LuongCtGiamTruSauThue>
    {
        public int LuongCtGiamTruSauThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal KinhPhiCongDoan { get; set; }
        public decimal QuyetToanThueTncn { get; set; }
        public decimal Other { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public decimal Sum { get; set; }

        public LuongCtGiamTruSauThueModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtGiamTruSauThueModel(LuongCtGiamTruSauThue entity)
        {
            Sum = entity.KinhPhiCongDoan + entity.QuyetToanThueTncn + entity.Other;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtGiamTruSauThue ToEntity()
        {
            var entity = new LuongCtGiamTruSauThue();
            Mapper(this, entity);
            return entity;
        }
    }
}
