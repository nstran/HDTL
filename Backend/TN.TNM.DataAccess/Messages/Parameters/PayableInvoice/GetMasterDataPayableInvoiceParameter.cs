using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class GetMasterDataPayableInvoiceParameter:BaseParameter
    {
       public Guid? VendorOrderId { get; set; }
    }
}
