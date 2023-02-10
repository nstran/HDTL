using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class VacanciesDocument
    {
        public Guid VacanciesDocumentId { get; set; }
        public Guid VacanciesId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }
        public string DocumentUrl { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
