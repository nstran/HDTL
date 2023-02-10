using System;
using System.Linq;
using TN.TNM.DataAccess.Databases;

namespace TN.TNM.DataAccess.Helper
{
    public static class AccessHelper
    {       
        public static bool GetAccessDataOfOrganization(TNTN8Context context, Guid userId)
        {
            var user = context.User.FirstOrDefault(x => x.UserId == userId);

            if (user != null)
            {
                var orgIdOfEmp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId)?.OrganizationId;             
                return context.Organization.FirstOrDefault(x => x.Active == true && x.OrganizationId == orgIdOfEmp).IsAccess == null ? false : true;
            }
            return false;
        }
    }
}
