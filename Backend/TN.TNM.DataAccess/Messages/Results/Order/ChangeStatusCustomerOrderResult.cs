using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class ChangeStatusCustomerOrderResult: BaseResult
    {
        public List<Guid> ListEmpId { get; set; }

    }
}
