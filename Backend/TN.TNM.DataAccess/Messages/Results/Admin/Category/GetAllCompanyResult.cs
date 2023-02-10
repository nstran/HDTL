using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Company;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Category
{
    public class GetAllCompanyResult : BaseResult
    {
        public List<CompanyEntityModel> Company { get; set; }
    }
}
