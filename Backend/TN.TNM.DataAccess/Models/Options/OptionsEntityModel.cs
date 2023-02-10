using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters;
using TN.TNM.DataAccess.Messages.Results.Options;

namespace TN.TNM.DataAccess.Models.Options
{
    public class OptionsEntityModel : BaseModel<DataAccess.Databases.Entities.Options>
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string NameCustom { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public bool? EnableNumber { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal? Price { get; set; }
        public Guid ProductId { get; internal set; }
        public Guid? ParentId { get; set; }
        public bool HasChild { get; set; }
        public string ServiceTypeName { get; set; }
        public Guid? ServicePacketId { get; set; }
        public decimal? VAT { get; set; }
        public bool? IsExtend { get; set; }

        public List<SearchOptionTree> ListOptionTrees { get; set; }


        public OptionsEntityModel()
        {
        }
        public override Databases.Entities.Options ToEntity()
        {
            var entity = new Databases.Entities.Options();
            Mapper(this, entity);
            return entity;
        }
        public OptionsEntityModel(Databases.Entities.Options model)
        {
            Mapper(model, this);
        }
    }
}
