using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class CreateOrUpdateCustomerOrderActionResult: BaseResult
    {
        public Guid? CustomerOrderActionId { get; set; }
        public List<Guid> ListEmpId { get; set; }

    }
}
