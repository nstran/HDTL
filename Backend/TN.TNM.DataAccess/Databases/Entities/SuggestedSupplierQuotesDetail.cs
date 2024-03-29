﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SuggestedSupplierQuotesDetail
    {
        public Guid SuggestedSupplierQuoteDetailId { get; set; }
        public Guid SuggestedSupplierQuoteId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
