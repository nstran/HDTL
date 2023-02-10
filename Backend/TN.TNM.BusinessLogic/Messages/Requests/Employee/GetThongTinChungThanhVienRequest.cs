using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetThongTinChungThanhVienRequest : BaseRequest<GetThongTinChungThanhVienParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetThongTinChungThanhVienParameter ToParameter()
        {
            return new GetThongTinChungThanhVienParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
