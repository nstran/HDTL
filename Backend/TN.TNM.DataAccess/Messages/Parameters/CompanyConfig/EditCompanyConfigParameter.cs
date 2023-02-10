using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Company;

namespace TN.TNM.DataAccess.Messages.Parameters.CompanyConfig
{
    public class EditCompanyConfigParameter : BaseParameter
    {
        public CompanyConfigurationEntityModel CompanyConfigurationObject { get; set; }
    }
}
