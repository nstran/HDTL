using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetThongTinNhanSuRequest : BaseRequest<GetThongTinNhanSuParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetThongTinNhanSuParameter ToParameter()
        {
            return new GetThongTinNhanSuParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
