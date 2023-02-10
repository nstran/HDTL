using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
     public class GetMasterCreateOrUpdateDeXuatCongTacResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<Position> ListPosition { get; set; }         
        public List<Organization> ListOrganization { get; set; }
    }    
}
