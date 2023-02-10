using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetMasterDataBaoCaoSuDungNguonLucResult : BaseResult
    {
        public List<ProjectEntityModel> ListProject { get; set; }
    }
}
