using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class TakeListOrganizationToFilterEmployeeResult : BaseResult
    {
       public List<OrganizationEntityModel> ListOrganizationEntityModel { get; set; }
    }
}
