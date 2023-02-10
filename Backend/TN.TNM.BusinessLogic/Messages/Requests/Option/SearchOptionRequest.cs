using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.Common;
using TN.TNM.DataAccess.Messages.Parameters.Options;

namespace TN.TNM.BusinessLogic.Messages.Requests.Option
{
    public class SearchOptionRequest : BaseRequest<SearchOptionParameter>
    {
        public Guid OptionId { get; set; }
        public string OptionName { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public Guid ParentId { get; set; }
        public override SearchOptionParameter ToParameter()
        {
            return new SearchOptionParameter()
            {
                OptionId = OptionId,
                OptionName = OptionName,
                Description = Description,
                Price = Price,
                ParentId = ParentId,
            };
        }
    }
}
