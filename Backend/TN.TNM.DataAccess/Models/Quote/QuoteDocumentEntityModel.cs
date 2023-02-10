using System;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteDocumentEntityModel : BaseModel<Databases.Entities.QuoteDocument>
    {
        public Guid QuoteDocumentId { get; set; }
        public Guid QuoteId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }
        public string DocumentUrl { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string UploadByName { get; set; }
        public QuoteDocumentEntityModel() { }

        public QuoteDocumentEntityModel(Databases.Entities.QuoteDocument model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.QuoteDocument ToEntity()
        {
            var entity = new Databases.Entities.QuoteDocument();
            Mapper(this, entity);
            return entity;
        }
    }
}
