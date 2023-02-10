using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtTroCapOtModel : BaseModel<LuongCtTroCapOt>
    {
        public int LuongCtTroCapOtId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal MucLuongHienTai { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }

        public LuongCtTroCapOtModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtTroCapOtModel(LuongCtTroCapOt entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtTroCapOt ToEntity()
        {
            var entity = new LuongCtTroCapOt();
            Mapper(this, entity);
            return entity;
        }
    }
}
