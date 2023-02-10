using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Options
{
    public class GetOptionCategoryResult : BaseResult
    {
        public List<OptionCategory> OptionCategory { get; set; }
    }
    public class OptionCategory
    {
        public string CategoryName { get; set; }
        public Guid CategoryId { get; set; }
    }
}
