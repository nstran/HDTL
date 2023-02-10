using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeInsuranceEntityModel:BaseModel<EmployeeInsurance>
    {
        public Guid EmployeeInsuranceId { get; set; }
        public Guid? EmployeeId { get; set; }
        public decimal? SocialInsuranceSalary { get; set; }
        public decimal? SocialInsuranceSupportPercent { get; set; }
        public decimal? HealthInsuranceSupportPercent { get; set; }
        public decimal? UnemploymentinsuranceSupportPercent { get; set; }
        public decimal? SocialInsurancePercent { get; set; }
        public decimal? HealthInsurancePercent { get; set; }
        public decimal? UnemploymentinsurancePercent { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public Guid? UpdateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        public Guid? TenantId { get; set; }

        public override EmployeeInsurance ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new EmployeeInsurance();
            Mapper(this, entity);
            return entity;
        }

        public EmployeeInsuranceEntityModel()
        {
        }

        public EmployeeInsuranceEntityModel(EmployeeInsurance entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }
    }
}
