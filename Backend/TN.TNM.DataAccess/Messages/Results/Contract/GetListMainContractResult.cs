using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class GetListMainContractResult : BaseResult
    {
        public List<ContractEntityModel> ListContract { get; set; }
    }
}
