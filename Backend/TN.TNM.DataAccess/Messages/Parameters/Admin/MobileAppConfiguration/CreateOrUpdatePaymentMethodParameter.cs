using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.MobileAppConfiguration
{
    public class CreateOrUpdatePaymentMethodParameter: BaseParameter
    {
        public PaymentMethodConfigureEntityModel Payment { get; set; }
    }
}
