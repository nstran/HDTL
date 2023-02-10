using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Options
{
    public class SearchOptionParameter : BaseParameter
    {
        public Guid OptionId { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public Guid ParentId { get; set; }
        public string OptionName { get; set; }
        public decimal? fromPrice { get; set; }
        public decimal? toPrice { get; set; }
        public string CategoryName { get; set; }
        public decimal? VAT { get; set; }
        public List<Guid> ListCategoryId { get; set; }

    }
}
