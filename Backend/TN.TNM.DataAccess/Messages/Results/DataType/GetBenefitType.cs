using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Messages.Results.DataType
{
    public class GetBenefitType : BaseResult
    {
        public List<BaseType> GetListBenefitType { get; set; }
    }
}
