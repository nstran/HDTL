using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetThongTinGhiChuRequest : BaseRequest<GetThongTinGhiChuParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetThongTinGhiChuParameter ToParameter()
        {
            return new GetThongTinGhiChuParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
