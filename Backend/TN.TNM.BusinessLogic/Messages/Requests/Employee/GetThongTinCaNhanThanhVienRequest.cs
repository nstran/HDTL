using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetThongTinCaNhanThanhVienRequest : BaseRequest<GetThongTinCaNhanThanhVienParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetThongTinCaNhanThanhVienParameter ToParameter()
        {
            return new GetThongTinCaNhanThanhVienParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
