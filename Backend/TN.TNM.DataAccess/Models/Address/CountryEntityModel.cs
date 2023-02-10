using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Address
{
    public class CountryEntityModel : BaseModel<Country>
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public CountryEntityModel()
        {

        }

        public CountryEntityModel(Country entity)
        {
            Mapper(entity, this);
        }

        public override Country ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Country();
            Mapper(this, entity);
            return entity;
        }
    }
}
