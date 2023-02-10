using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin
{
    public class CreateOrUpdatePaymentMethodResult: BaseResult
    {
        public PaymentMethodConfigureEntityModel Payment { get; set; }
    }
}
