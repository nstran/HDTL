using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class ResetPasswordRequest : BaseRequest<ResetPasswordParameter>
    {
        public Guid EmployeeId { get; set; }
        public override ResetPasswordParameter ToParameter()
        {
            return new ResetPasswordParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
