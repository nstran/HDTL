using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Folder;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class CreateOrUpdateContractParameter : BaseParameter
    {
        public ContractEntityModel Contract { get; set; }
        public List<ContractDetailEntityModel> ContractDetail { get; set; }
        public List<AdditionalInformationEntityModel> ListAdditionalInformation { get; set; }
        public List<ContractCostDetailEntityModel> ListContractCost { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<FileInFolderEntityModel> ListFile { get; set; }
        public Boolean IsCreate { get; set; }

    }
}
