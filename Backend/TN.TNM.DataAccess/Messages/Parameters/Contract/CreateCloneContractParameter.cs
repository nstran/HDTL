using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class CreateCloneContractParameter : BaseParameter
    {
        public ContractEntityModel Contract { get; set; }
        public List<ContractDetailEntityModel> ContractDetail { get; set; }
        public List<AdditionalInformationEntityModel> ListAdditionalInformation { get; set; }
        public List<ContractCostDetailEntityModel> ListContractCost { get; set; }
        public Guid ContractId { get; set; }
        public bool? Check { get; set; }
    }
}
