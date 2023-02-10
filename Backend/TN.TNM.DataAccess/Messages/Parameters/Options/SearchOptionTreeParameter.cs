using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Options
{
    public class SearchOptionTreeParameter : BaseParameter
    {
        public Guid OptionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public Guid ParentId { get; set; }
    }
}
