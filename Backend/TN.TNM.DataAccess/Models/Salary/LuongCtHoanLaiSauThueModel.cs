using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtHoanLaiSauThueModel : BaseModel<LuongCtHoanLaiSauThue>
    {
        public int LuongCtHoanLaiSauThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal ThueTncn { get; set; }
        public decimal Other { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public decimal Sum { get; set; }

        public LuongCtHoanLaiSauThueModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtHoanLaiSauThueModel(LuongCtHoanLaiSauThue entity)
        {
            Sum = entity.ThueTncn + entity.Other;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtHoanLaiSauThue ToEntity()
        {
            var entity = new LuongCtHoanLaiSauThue();
            Mapper(this, entity);
            return entity;
        }
    }
}
