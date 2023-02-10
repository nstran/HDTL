using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeAllowanceEntityModel:BaseModel<EmployeeAllowance>
    {
        public Guid EmployeeAllowanceId { get; set; }
        public decimal? LunchAllowance { get; set; }
        public decimal? MaternityAllowance { get; set; }
        public decimal? FuelAllowance { get; set; }
        public decimal? PhoneAllowance { get; set; }
        public decimal? OtherAllownce { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool? FreeTimeUnlimited { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public Guid? TenantId { get; set; }
        public EmployeeAllowanceEntityModel() { }

        public EmployeeAllowanceEntityModel(EmployeeAllowance entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public override EmployeeAllowance ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new EmployeeAllowance();
            Mapper(this, entity);
            return entity;
        }
    }
}
