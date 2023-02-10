using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CustomerServiceLevel
{
    public class CustomerServiceLevelEntityModel : BaseModel<DataAccess.Databases.Entities.CustomerServiceLevel>
    {
        public Guid? CustomerServiceLevelId { get; set; }
        public string CustomerServiceLevelName { get; set; }
        public string CustomerServiceLevelCode { get; set; }
        public decimal? MinimumSaleValue { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public CustomerServiceLevelEntityModel()
        {

        }

        public CustomerServiceLevelEntityModel(DataAccess.Databases.Entities.CustomerServiceLevel entity)
        {
            Mapper(entity, this);
        }

        public override Databases.Entities.CustomerServiceLevel ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.CustomerServiceLevel();
            Mapper(this, entity);
            return entity;
        }
    }
}
