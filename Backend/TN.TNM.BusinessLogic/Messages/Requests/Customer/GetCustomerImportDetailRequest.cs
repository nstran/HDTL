﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetCustomerImportDetailRequest:BaseRequest<GetCustomerImportDetailParameter>
    {
        public override GetCustomerImportDetailParameter ToParameter()
        {
            return new GetCustomerImportDetailParameter
            {

            };
        }
    }
}
