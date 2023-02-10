using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Company
{
    public class CompanyEntityModel : BaseModel<DataAccess.Databases.Entities.Company>
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public CompanyEntityModel(DataAccess.Databases.Entities.Company entity) : base(entity)
        {
            Mapper(this, entity);
        }
        public CompanyEntityModel()
        {
           
        }
        public override DataAccess.Databases.Entities.Company ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Company();
            Mapper(this, entity);
            return entity;
        }
    }
}
