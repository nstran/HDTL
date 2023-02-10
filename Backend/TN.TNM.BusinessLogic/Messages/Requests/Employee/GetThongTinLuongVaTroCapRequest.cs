using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetThongTinLuongVaTroCapRequest : BaseRequest<GetThongTinLuongVaTroCapParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetThongTinLuongVaTroCapParameter ToParameter()
        {
            return new GetThongTinLuongVaTroCapParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
