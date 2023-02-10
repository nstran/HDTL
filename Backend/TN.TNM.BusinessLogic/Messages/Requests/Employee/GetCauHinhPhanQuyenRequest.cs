using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetCauHinhPhanQuyenRequest : BaseRequest<GetCauHinhPhanQuyenParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetCauHinhPhanQuyenParameter ToParameter()
        {
            return new GetCauHinhPhanQuyenParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
