using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Options
{
    public class GetListCategoryAttributesByIdParameter : BaseParameter
    {
        public Guid Id { get; set; }
    }
}
